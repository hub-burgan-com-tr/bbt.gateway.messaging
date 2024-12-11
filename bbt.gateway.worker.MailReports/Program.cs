using bbt.gateway.common;
using bbt.gateway.common.Api.MessagingGateway;
using bbt.gateway.common.Helpers;
using bbt.gateway.worker.MailReports;
using Microsoft.EntityFrameworkCore;
using Refit;

IHost host = Host.CreateDefaultBuilder(args)
    .UseVaultSecrets(typeof(Program))
    .UseSeriLog("entegrasyon")
    .ConfigureServices((context, services) =>
    {
        services.AddAllElasticApm();

        services.AddRefitClient<IMessagingGatewayApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(context.Configuration["Api:ServiceUrl"]));

        services.AddHostedService<MailWorker>();

        services.AddSingleton<DbContextOptions<DatabaseContext>>(new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"))
                .Options);

        services.AddSingleton<LogManager>();
    })
    .Build();

await host.RunAsync();