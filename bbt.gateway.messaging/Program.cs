
using bbt.gateway.common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace bbt.gateway.messaging
{

    public class Program
    {

        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(50,50);
            Host.CreateDefaultBuilder(args)
            .UseVaultSecrets(typeof(Program))
            .UseSeriLog("entegrasyon")
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).Build().Run();

        }

    }
}
