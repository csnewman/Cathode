using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Cathode.Gateway.Certificates
{
    public class DevelopmentCertificateService : BackgroundService
    {
        private const string AspNetHttpsOid = "1.3.6.1.4.1.311.84.1.1";
        private readonly ICertificateStore _certStore;

        public DevelopmentCertificateService(ICertificateStore certStore)
        {
            _certStore = certStore;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
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