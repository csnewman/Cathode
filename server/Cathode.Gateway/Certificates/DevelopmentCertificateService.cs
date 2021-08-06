using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cathode.Gateway.Certificates
{
    public class DevelopmentCertificateService : BackgroundService
    {
        private const string AspNetHttpsOid = "1.3.6.1.4.1.311.84.1.1";
        private readonly ICertificateStore _certStore;
        private readonly ILogger<DevelopmentCertificateService> _logger;

        public DevelopmentCertificateService(ICertificateStore certStore, ILogger<DevelopmentCertificateService> logger)
        {
            _certStore = certStore;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Searching for development certificates");
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindByExtension, AspNetHttpsOid, validOnly: false);

            foreach (var cert in certs)
            {
                _certStore.AddCertificate(cert);
            }

            return Task.CompletedTask;
        }
    }
}