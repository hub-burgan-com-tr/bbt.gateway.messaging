using bbt.gateway.common;
using bbt.gateway.common.Api.dEngage;
using bbt.gateway.common.Helpers;
using bbt.gateway.worker;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Refit;
using System.Text.Json;

var host = Host.CreateDefaultBuilder()
    .UseConsulSettings(typeof(Program))
    .UseSeriLog("entegrasyon")
    .ConfigureServices((context, services) =>
    {
        services.AddDaprClient(builder =>
                   builder.UseJsonSerializationOptions(
                       new JsonSerializerOptions()
                       {
                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                           PropertyNameCaseInsensitive = true,
                       }));

        services.AddRefitClient<IdEngageClient>(new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        }
                )
        })
               .ConfigureHttpClient(c => c.BaseAddress = new Uri(context.Configuration["Api:dEngage:BaseAddress"]));

        services.AddHostedService<TemplateWorker>();

        services.AddSingleton<DbContextOptions<DatabaseContext>>(new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")).UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()))
                .Options);
        services.AddSingleton<LogManager>();

    })
    .UseAllElasticApm()
    .Build();

await host.RunAsync();
