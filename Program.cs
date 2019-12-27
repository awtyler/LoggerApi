using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace log_webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
		    webBuilder.UseUrls("https://home.tylerinternet.com:5001;http://home.tylerinternet.com:5000");
                    webBuilder.ConfigureKestrel(serverOptions => 
                    {
                        serverOptions.ConfigureHttpsDefaults(listenOptions => 
                        {
                            X509Certificate2 certificate = new X509Certificate2("/etc/letsencrypt/live/home.tylerinternet.com/x509cert2.pfx");
                            listenOptions.ServerCertificate = certificate;
                        });
                    });
                });
    }
}
