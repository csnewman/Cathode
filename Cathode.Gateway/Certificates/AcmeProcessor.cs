using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Cathode.Common.Settings;
using Certes;
using Microsoft.Extensions.Logging;

namespace Cathode.Gateway.Certificates
{
    public class AcmeProcessor : IAcmeProcessor
    {
        private readonly ILogger<AcmeProcessor> _logger;
        private readonly GatewayOptions _options;
        private readonly ISettingsProvider<GatewayDb, GatewaySetting> _settingsProvider;
        private readonly IAcmeManager _acmeManager;
        private readonly GatewayDb _db;
        private readonly ICertificateStore _certStore;

        public AcmeProcessor(ILogger<AcmeProcessor> logger, GatewayOptions options,
            ISettingsProvider<GatewayDb, GatewaySetting> settingsProvider, IAcmeManager acmeManager, GatewayDb db,
            ICertificateStore certStore)
        {
            _logger = logger;
            _options = options;
            _settingsProvider = settingsProvider;
            _acmeManager = acmeManager;
            _db = db;
            _certStore = certStore;
        }

        public async Task CheckCertificateAsync()
        {
            _logger.LogInformation("Checking ACME state");

            _certStore.PruneCertificates();

            // Renew certificates if needed
            Debug.Assert(_options.AcmeDomain != null, "_options.AcmeDomain != null");
            var cert = _certStore.GetCertificate(_options.AcmeDomain);

            if (!IsCertificateValid(cert))
            {
                await RenewCertificateAsync();
            }
        }

        public async Task LoadCertificateAsync()
        {
            var settings = await _settingsProvider.GetOrAddAsync(AcmeSettings.Id, () => new AcmeSettings());

            if (settings.Certificate == null || !string.Equals(_options.AcmeEmail, settings.Email) ||
                !string.Equals(_options.AcmeServer, settings.Server) ||
                !string.Equals(_options.AcmeDomain, settings.Domain))
            {
                _logger.LogInformation("No ACME certificate found");
                return;
            }

            var cert = new X509Certificate2(settings.Certificate);
            if (IsCertificateValid(cert))
            {
                _logger.LogInformation("Loaded ACME certificate");
                _certStore.AddCertificate(cert);
            }
            else
            {
                _logger.LogInformation("Existing ACME certificate invalid. Ignoring");
            }
        }

        private static bool IsCertificateValid(X509Certificate2? cert)
        {
            return cert != null && (cert.NotAfter - DateTime.Now).TotalDays >= 30;
        }

        private async Task RenewCertificateAsync(bool reuseAccount = true)
        {
            _logger.LogInformation("Renewing ACME certificate for {}", _options.AcmeDomain);

            Debug.Assert(_options.AcmeServer != null, "_options.AcmeServer != null");
            Debug.Assert(_options.AcmeDomain != null, "_options.AcmeDomain != null");
            Debug.Assert(_options.AcmeEmail != null, "_options.AcmeEmail != null");

            var settings = await _settingsProvider.GetOrAddAsync(AcmeSettings.Id, () => new AcmeSettings());

            // Load account
            IAcmeContext acme;
            if (!reuseAccount || settings.AccountKey == null || !string.Equals(_options.AcmeEmail, settings.Email) ||
                !string.Equals(_options.AcmeServer, settings.Server))
            {
                _logger.LogInformation("Registering new ACME account");
                acme = new AcmeContext(new Uri(_options.AcmeServer));
                await acme.NewAccount(_options.AcmeEmail, true);
                settings.AccountKey = acme.AccountKey.ToDer();
                settings.Server = _options.AcmeServer;
                settings.Email = _options.AcmeEmail;

                await _db.SaveChangesAsync();
            }
            else
            {
                try
                {
                    _logger.LogInformation("Loading ACME account");
                    var accountKey = KeyFactory.FromDer(settings.AccountKey);
                    acme = new AcmeContext(new Uri(_options.AcmeServer), accountKey);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to load ACME account");
                    await RenewCertificateAsync(false);
                    return;
                }
            }

            // Place order
            var order = await acme.NewOrder(new[] { _options.AcmeDomain });

            // Validate authenticity
            var auth = (await order.Authorizations()).First();
            var challenge = await auth.Http();
            _acmeManager.StoreChallenge(challenge.Token, challenge.KeyAuthz);
            _logger.LogInformation("Challenge response stored for {}", challenge.Token);
            await challenge.Validate();

            // Request certificate
            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
            var cert = await order.Generate(new CsrInfo
            {
                Organization = "Cathode",
                OrganizationUnit = "Gateway",
                CommonName = _options.AcmeDomain,
            }, privateKey);
            var pfxBuilder = cert.ToPfx(privateKey);
            var pfx = pfxBuilder.Build(_options.AcmeDomain + " Certificate", string.Empty);
            var x509 = new X509Certificate2(pfx, string.Empty, X509KeyStorageFlags.Exportable);

            settings.Domain = _options.AcmeDomain;
            settings.Certificate = x509.Export(X509ContentType.Pkcs12);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Generated certificate from ACME provider");

            _certStore.AddCertificate(x509);
        }
    }
}