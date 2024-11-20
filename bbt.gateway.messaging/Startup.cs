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
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.messaging.Api.Infobip;
using System.Linq;
using bbt.gateway.common.Api.Reminder;
using bbt.gateway.messaging.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Configuration;
using Google.Apis.Http;
using bbt.gateway.common.Http;
using bbt.gateway.common.Api.Amorphie;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using bbt.gateway.common.GlobalConstants;
using bbt.gateway.common.Models.v2;

namespace bbt.gateway.messaging
{
    public delegate IVodafoneApi VodafoneApiFactory(bool useFakeSmtp);
    public delegate ITurkcellApi TurkcellApiFactory(bool useFakeSmtp);
    public delegate ITurkTelekomApi TurkTelekomApiFactory(bool useFakeSmtp);
    public delegate IOperatordEngage dEngageFactory(bool useFakeSmtp);
    public delegate IOperatorCodec CodecFactory(bool useFakeSmtp);
    public delegate IOperatorInfobip InfobipFactory(bool useFakeSmtp);

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
            
            //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Security:Key"]));
            //var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            //var jwtSecurityToken = new JwtSecurityToken(
            //        issuer: "XXX",
            //        audience: "XXXX",
            //        claims: new List<Claim>(),
            //        expires: DateTime.Now.AddMinutes(1),
            //        signingCredentials: signinCredentials
            //    );
            //var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = (context) =>
                    {
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnMessageReceived = (context) => {

                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Configuration["Security:Issuer"],
                    ValidAudiences = Configuration.GetSection("Security:Audiences").Get<IEnumerable<string>>(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Security:Key"]))
                };
            });

            services.AddDaprClient(builder =>
               builder.UseJsonSerializationOptions(
                   new JsonSerializerOptions()
                   {
                       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                       PropertyNameCaseInsensitive = true,
                   }));

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
                v.DefaultApiVersion = new ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddApiExplorer(setup =>
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
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please Enter a Valid Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    { 
                        new OpenApiSecurityScheme
                        { 
                            Reference = new OpenApiReference
                            { 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }    
                });
                c.DocumentFilter<PathDocumentFilter>();
            });
            
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

        
            
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = $"{Configuration["Redis:Host"]}:{Configuration["Redis:Port"]},password={Configuration["Redis:Password"]}";
            });
            
            services.AddDbContext<DatabaseContext>(o => { o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("bbt.gateway.messaging"));});
            //services.AddDbContext<DodgeDatabaseContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DodgeConnection")));
            services.AddDbContext<SmsBankingDatabaseContext>(o => o.UseSqlServer(Configuration.GetConnectionString("SmsBankingConnection")));
            services.Configure<UserSettings>(Configuration.GetSection(nameof(UserSettings)));
            services.Configure<common.Models.v2.InboxExpireSettings>(Configuration.GetSection(nameof(common.Models.v2.InboxExpireSettings)));
            services.AddScoped<IRepositoryManager, RepositoryManager>();

            services.AddRefitClient<IMessagingGatewayApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["Api:MessagingGateway:BaseAddress"]));

            services.AddRefitClient<IUserApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["Api:Amorphie:User:BaseAddress"]));

            services.AddRefitClient<IUserApiPrep>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["Api:Amorphie:User:PrepBaseAddress"]));

            services.AddRefitClient<IReminderApi>()
            .ConfigureHttpClient(c => {
                c.BaseAddress = new Uri(Configuration["Api:Reminder:BaseAddress"]);
                c.DefaultRequestHeaders.Add("channel", Configuration["Api:Reminder:Channel"]);
                c.DefaultRequestHeaders.Add("branch", Configuration["Api:Reminder:Branch"]);
                c.DefaultRequestHeaders.Add("user", Configuration["Api:Reminder:User"]);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    UseProxy = false,
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };
            });

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

            services.AddHttpClient("foraClient", httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("channel", Configuration["Api:Fora:Channel"]);
                httpClient.DefaultRequestHeaders.Add("branch", Configuration["Api:Fora:Branch"]);
                httpClient.DefaultRequestHeaders.Add("user", Configuration["Api:Fora:User"]);
            });

            var factory = new ProxyByPassHttpClientFactory();
            GoogleCredential credential = GoogleCredential.FromJson(Configuration["Firebase"]);
            credential = credential.CreateWithHttpClientFactory(factory);
            FirebaseApp.Create(new AppOptions()
            {
                Credential = credential,
                HttpClientFactory = factory
            });

            services.AddAllElasticApm();

            services.AddScoped<IOperatorService, OperatorService>();
            services.AddScoped<InstantReminder>();
            services.AddScoped<IFakeSmtpHelper, FakeSmtpHelper>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<OperatorTurkTelekom>();
            services.AddScoped<OperatorVodafone>();
            services.AddScoped<OperatorTurkcell>();
            services.AddScoped<OperatorIVN>();
            services.AddScoped<OperatordEngage>();
            services.AddScoped<OperatordEngageMock>();
            services.AddScoped<IOperatorFirebase,OperatorFirebase>();
            services.AddScoped<OperatorCodec>();
            services.AddScoped<OperatorCodecMock>();
            services.AddScoped<OperatorInfobip>();
            services.AddScoped<OperatorInfobipMock>();
            services.AddScoped<TurkTelekomApi>();
            services.AddScoped<VodafoneApi>();
            services.AddScoped<TurkcellApi>();
            services.AddScoped<IInfobipApi,InfobipApi>();
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
            services.AddScoped<CodecFactory>(serviceProvider => useFakeSmtp =>
            {
                return useFakeSmtp switch
                {
                    true => serviceProvider.GetRequiredService<OperatorCodecMock>(),
                    _ => serviceProvider.GetRequiredService<OperatorCodec>()
                };
            });
            services.AddScoped<InfobipFactory>(serviceProvider => useFakeSmtp =>
            {
                return useFakeSmtp switch
                {
                    true => serviceProvider.GetRequiredService<OperatorInfobipMock>(),
                    _ => serviceProvider.GetRequiredService<OperatorInfobip>()
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

            services.AddScoped<InfobipSender>();
            services.AddScoped<OtpSender>();
            services.AddScoped<dEngageSender>();
            services.AddScoped<FirebaseSender>();
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
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Mock")
            {
                app.UseGatewayMiddleware();
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

            app.UseCloudEvents();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {                
                endpoints.MapControllers();
                endpoints.MapSubscribeHandler();
            });
        }
    }
}