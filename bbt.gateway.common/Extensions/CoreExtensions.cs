using Consul;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using VaultSharp.Extensions.Configuration;
using Winton.Extensions.Configuration.Consul;

namespace bbt.gateway.common
{
    public static class CoreExtensions
    {
        /// <summary>
        /// Read Appsettings From Consule<br />
        /// ConsulHost and ConsulToken have to be set in appsettings
        /// </summary>
        /// <param name="host"></param>
        /// <param name="type">Type of Main | usage : typeof(Program)</param>
        /// <param name="fullPath">secret path</param>
        /// <returns></returns>
        public static IHostBuilder UseConsulSettings(this IHostBuilder host, Type type, string fullPath = null)
        {


            return host.ConfigureAppConfiguration((context, builder) =>
            {
                string applicationName = fullPath ?? context.HostingEnvironment.ApplicationName;
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

                builder.AddJsonFile($"appsettings.{environmentName}.json", false, true)
                .AddUserSecrets(type.Assembly).AddEnvironmentVariables();

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
                    return;

                context.Configuration = builder.Build();

                string consulHost = context.Configuration["ConsulHost"];

                void ConsulConfig(ConsulClientConfiguration configuration)
                {
                    configuration.Token = context.Configuration["ConsulToken"];
                    configuration.Address = new Uri(consulHost);
                }

                builder.AddConsul($"{applicationName}/appsettings.json",
                    source =>
                    {
                        source.ReloadOnChange = true;
                        source.ConsulConfigurationOptions = ConsulConfig;
                    });
                builder.AddConsul($"{applicationName}/appsettings.{environmentName}.json",
                    source =>
                    {
                        source.Optional = true;
                        source.ConsulConfigurationOptions = ConsulConfig;
                    });

            });

        }

        /// <summary>
        /// Set Serilog Configuration To Logging Elastic Search<br />
        /// ElasticSearch:ApiKey and ElasticSearch:Url have to be set in appsettings
        /// </summary>
        /// <param name="host"></param>
        /// <param name="indexFormat">Index Format for Elastic Search</param>
        /// <returns></returns>
        public static IHostBuilder UseSeriLog(this IHostBuilder host, string indexFormat)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Mock")
                return host;

            return host.ConfigureAppConfiguration((context, builder) =>
            {
                
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

                var configuration = builder.Build();
                
                ApiKeyAuthenticationCredentials k = new ApiKeyAuthenticationCredentials(configuration["ElasticSearch:ApiKey"]);
                indexFormat = (environmentName != "Prod" ? ( environmentName != "Drc" ? "nonprod-" : "drc") : "prod-") + indexFormat;
                Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]))
                {
                    IndexFormat = indexFormat + "-{0:yyyy-MM}",
                    ModifyConnectionSettings = c => c.ApiKeyAuthentication(k),
                    TypeName = null
                })
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            }).UseSerilog();

        }



        public static IHostBuilder UseVaultSecrets(this IHostBuilder host, Type type)
        {
            return host.ConfigureAppConfiguration((context, builder) =>
            {
                using var loggerFactory = LoggerFactory.Create(builder =>
                    builder.AddConsole(c => c.LogToStandardErrorThreshold = Microsoft.Extensions.Logging.LogLevel.Debug).AddDebug());
                var logger = loggerFactory.CreateLogger<TestLog>();

                string applicationName = context.HostingEnvironment.ApplicationName;
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                builder.AddJsonFile($"appsettings.{environmentName}.json", false, true).AddUserSecrets(type.Assembly);

                var conf = builder.Build();

                builder.AddVaultConfiguration(() => new VaultOptions(conf["Api:Vault:BaseAddress"], conf["Api:Vault:Token"]), $"{applicationName}.{environmentName}", "MessagingGateway", logger);
            });
        }


    }

    public class TestLog
    { }
}
