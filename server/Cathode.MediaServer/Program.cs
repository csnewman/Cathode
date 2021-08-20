using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cathode.MediaServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Cathode Media Server");

            // Apply db migrations
            using (var scope = host.Services.CreateScope())
            {
                logger.LogInformation("Checking database migrations");
                var db = scope.ServiceProvider.GetRequiredService<ServerDb>();

                db.Database.Migrate();
            }


            logger.LogInformation("Starting server");
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
                        // var resolver = options.ApplicationServices.GetRequiredService<ICertificateStore>();
                        // options.ConfigureHttpsDefaults(opt =>
                        // {
                        //     opt.ServerCertificateSelector = resolver.SelectCertificate;
                        // });

                        options.AddServerHeader = false;
                    });
                    webBuilder.UseUrls("http://*:34445", "https://*:34446");
                    webBuilder.UseStartup<Startup>();
                });
    }
}