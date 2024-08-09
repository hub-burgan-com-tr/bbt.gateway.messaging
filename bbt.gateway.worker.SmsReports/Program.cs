using System.Text.Json;
using bbt.gateway.common;
using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.Helpers;
using bbt.gateway.worker.SmsReports;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore;
using Refit;

IHost host = Host.CreateDefaultBuilder(args)
    .UseVaultSecrets(typeof(Program))
    .UseSeriLog("entegrasyon")
    .ConfigureServices((context, services) =>
    {
        services.AddRefitClient<IMessagingGatewayApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(context.Configuration["Api:ServiceUrl"]));
        services.AddHostedService<SmsWorker>();
        services.AddSingleton<DbContextOptions<DatabaseContext>>(new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"))
                .Options);
        services.AddSingleton<LogManager>();
        services.AddDaprClient(builder =>
                   builder.UseJsonSerializationOptions(
                       new JsonSerializerOptions()
                       {
                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                           PropertyNameCaseInsensitive = true,
                       }));
        services.AddScoped<KafkaHelper>();

    })
    .UseAllElasticApm()
    .Build();

await host.RunAsync();

