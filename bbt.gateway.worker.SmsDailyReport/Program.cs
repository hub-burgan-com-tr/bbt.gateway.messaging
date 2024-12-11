using bbt.gateway.common;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Repositories;
using bbt.gateway.worker.SmsDailyReport;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .UseVaultSecrets(typeof(Program))
    .UseSeriLog("entegrasyon")
    .ConfigureServices((context, services) =>
    {
        services.AddAllElasticApm();

        services.AddHostedService<SmsDailyReportWorker>();
        services.AddDbContext<DatabaseContext>(o =>  o.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Singleton);
        services.AddDbContext<SmsBankingDatabaseContext>(o => o.UseSqlServer(context.Configuration.GetConnectionString("SmsBankingConnection")),ServiceLifetime.Singleton);
        services.AddSingleton<IRepositoryManager, RepositoryManager>();
        services.AddSingleton<LogManager>();
        services.AddDaprClient();
    })
    .Build();

await host.RunAsync();