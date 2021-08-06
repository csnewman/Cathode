using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cathode.Gateway.Certificates
{
    public class AcmeService : BackgroundService
    {
        private ILogger<AcmeService> _logger;
        private readonly IServiceScopeFactory _factory;

        public AcmeService(IServiceScopeFactory factory, ILogger<AcmeService> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _factory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IAcmeProcessor>();

            await processor.LoadCertificateAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await processor.CheckCertificateAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to check ACME certificates");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}