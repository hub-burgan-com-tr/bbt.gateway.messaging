using bbt.gateway.common;
using bbt.gateway.common.Helpers;
using bbt.gateway.common.Repositories;
using bbt.gateway.worker.SmsDailyReport;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .UseVaultSecrets(typeof(Program))
    .UseSeriLog("entegrasyon")
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<SmsDailyReportWorker>();
        services.AddDbContext<DatabaseContext>(o =>  o.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddSingleton<IRepositoryManager, RepositoryManager>();
        services.AddSingleton<LogManager>();

    })
    .UseAllElasticApm()
    .Build();

await host.RunAsync();
