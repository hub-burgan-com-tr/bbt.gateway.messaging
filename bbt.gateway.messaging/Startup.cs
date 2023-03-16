using bbt.gateway.common;
using bbt.gateway.common.Models;
using bbt.gateway.common.Models.v1;
using bbt.gateway.common.Repositories;
using bbt.gateway.common.Api.dEngage;
using bbt.gateway.messaging.Api.Fora;
using bbt.gateway.messaging.Api.Pusula;
using bbt.gateway.messaging.Api.Turkcell;
using bbt.gateway.messaging.Api.TurkTelekom;
using bbt.gateway.messaging.Api.Vodafone;
using bbt.gateway.messaging.Helpers;
using bbt.gateway.messaging.Middlewares;
using bbt.gateway.messaging.Workers;
using bbt.gateway.messaging.Workers.OperatorGateway;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Refit;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace bbt.gateway.messaging
{
    public delegate IVodafoneApi VodafoneApiFactory(bool useFakeSmtp);
    public delegate ITurkcellApi TurkcellApiFactory(bool useFakeSmtp);
    public delegate ITurkTelekomApi TurkTelekomApiFactory(bool useFakeSmtp);
    public delegate IOperatordEngage dEngageFactory(bool useFakeSmtp);

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddControllers()
                    .AddNewtonsoftJson(opts =>
                    {
                        opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                        opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });

            JsonSerializerSettings settings = new();
            settings.Converters.Add(new StringEnumConverter());
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            JsonConvert.DefaultSettings = () => settings;

            services.AddApiVersioning(v =>
            {
                v.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.UseInlineDefinitionsForEnums();
                c.CustomSchemaIds(type => type.FullName);
                c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
                c.ExampleFilters();
             
                
                c.IncludeXmlComments("wwwroot/bbt.gateway.messaging.xml");
                c.IncludeXmlComments("wwwroot/bbt.gateway.common.xml");

                c.SwaggerDoc("v1", new()
                {
                    Version = "v1",
                    Title = "Bbt.Gateway.Messaging",
                });
                c.SwaggerDoc("v2", new()
                {
                    Version = "v2",
                    Title = "Bbt.Gateway.Messaging",
                });
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

            services.AddDaprClient(builder =>
                   builder.UseJsonSerializationOptions(
                       new JsonSerializerOptions()
                       {
                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                           PropertyNameCaseInsensitive = true,
                       }));

            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = $"{Configuration["Redis:Host"]}:{Configuration["Redis:Port"]},password={Configuration["Redis:Password"]}";
            });

            services.AddDbContext<DatabaseContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("bbt.gateway.messaging")));
            //services.AddDbContext<DodgeDatabaseContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DodgeConnection")));
            services.AddDbContext<SmsBankingDatabaseContext>(o => o.UseSqlServer(Configuration.GetConnectionString("SmsBankingConnection")));
            services.Configure<UserSettings>(Configuration.GetSection(nameof(UserSettings)));
            services.Configure<common.Models.v2.InboxExpireSettings>(Configuration.GetSection(nameof(common.Models.v2.InboxExpireSettings)));
            services.AddScoped<IRepositoryManager, RepositoryManager>();


            services.AddRefitClient<IdEngageClient>(new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        }
                )
                
            })
               .ConfigureHttpClient(c => 
               { 
                   c.BaseAddress = new Uri(Configuration["Api:dEngage:BaseAddress"]);
                   c.Timeout = TimeSpan.FromSeconds(200);
               });

            services.AddHttpClient("default", httpClient =>
            {

            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    UseProxy = false
                };
            });

            services.AddScoped<IFakeSmtpHelper, FakeSmtpHelper>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<OperatorTurkTelekom>();
            services.AddScoped<OperatorVodafone>();
            services.AddScoped<OperatorTurkcell>();
            services.AddScoped<OperatorIVN>();
            services.AddScoped<OperatordEngage>();
            services.AddScoped<OperatordEngageMock>();
            services.AddScoped<OperatorCodec>();
            services.AddScoped<TurkTelekomApi>();
            services.AddScoped<VodafoneApi>();
            services.AddScoped<TurkcellApi>();
            services.AddScoped<TurkTelekomApiMock>();
            services.AddScoped<VodafoneApiMock>();
            services.AddScoped<TurkcellApiMock>();
            services.AddScoped<TurkcellApiFactory>(serviceProvider => useFakeSmtp =>
            {
                return useFakeSmtp switch
                {
                    true => serviceProvider.GetRequiredService<TurkcellApiMock>(),
                    _ => serviceProvider.GetRequiredService<TurkcellApi>()
                };
            });
            services.AddScoped<VodafoneApiFactory>(serviceProvider => useFakeSmtp =>
            {
                return useFakeSmtp switch
                {
                    true => serviceProvider.GetRequiredService<VodafoneApiMock>(),
                    _ => serviceProvider.GetRequiredService<VodafoneApi>()
                };
            });
            services.AddScoped<TurkTelekomApiFactory>(serviceProvider => useFakeSmtp =>
            {
                return useFakeSmtp switch
                {
                    true => serviceProvider.GetRequiredService<TurkTelekomApiMock>(),
                    _ => serviceProvider.GetRequiredService<TurkTelekomApi>()
                };
            });
            services.AddScoped<dEngageFactory>(serviceProvider => useFakeSmtp =>
            {
                return useFakeSmtp switch
                {
                    true => serviceProvider.GetRequiredService<OperatordEngageMock>(),
                    _ => serviceProvider.GetRequiredService<OperatordEngage>()
                };
            });
            services.AddScoped<Func<OperatorType, IOperatorGateway>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case OperatorType.Turkcell:
                        return serviceProvider.GetService<OperatorTurkcell>();
                    case OperatorType.Vodafone:
                        return serviceProvider.GetService<OperatorVodafone>();
                    case OperatorType.TurkTelekom:
                        return serviceProvider.GetService<OperatorTurkTelekom>();
                    case OperatorType.IVN:
                        return serviceProvider.GetService<OperatorIVN>();
                    default:
                        throw new KeyNotFoundException();
                }
            });


            services.AddScoped<OtpSender>();
            services.AddScoped<dEngageSender>();
            services.AddScoped<CodecSender>();
            services.AddScoped<HeaderManager>();
            services.AddScoped<OperatorManager>();
            services.AddScoped<OperatorIVN>();
            services.AddScoped<PusulaClient>();
            services.AddScoped<ForaClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseAllElasticApm(Configuration);
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Mock")
            {
                app.UseTransactionMiddleware();
                app.UseCustomerInfoMiddleware();
                app.UseWhitelistMiddleware();
            }

            app.UseSwagger();
            app.UseStaticFiles();
            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet("Swagger.css");
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                    options.RoutePrefix = "";
                }
            });


            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                ResultStatusCodes =
            {
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
            });

            app.UseRouting();
           
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
            
        }
    }



}
