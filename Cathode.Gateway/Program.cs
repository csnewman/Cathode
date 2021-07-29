using Cathode.Gateway.Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cathode.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Apply db migrations
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GatewayDb>();
                db.Database.Migrate();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddEnvironmentVariables(options => { options.Prefix = "CATHODE_"; });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        var resolver = options.ApplicationServices.GetRequiredService<ICertificateStore>();
                        options.ConfigureHttpsDefaults(opt =>
                        {
                            opt.ServerCertificateSelector = resolver.SelectCertificate;
                        });

                        options.AddServerHeader = false;
                    });
                    webBuilder.UseUrls("http://*", "https://*");
                    webBuilder.UseStartup<Startup>();
                });
    }
}