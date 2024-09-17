using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using VaultSharp.Extensions.Configuration;

namespace bbt.gateway.common
{
    public static class CoreExtensions
    {
        

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
                
                //ApiKeyAuthenticationCredentials k = new ApiKeyAuthenticationCredentials(configuration["ElasticSearch:ApiKey"]);
                indexFormat = (environmentName != "Prod" ? ( environmentName != "Drc" ? "nonprod-" : "drc") : "prod-") + indexFormat;
                Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Elasticsearch([new Uri(configuration["ElasticSearch:Url"])], configureOptions : (o) => { o.DataStream = new DataStreamName(indexFormat); } ,configureTransport: (transport) => { transport.Authentication(new ApiKey(configuration["ElasticSearch:ApiKey"])); })
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
