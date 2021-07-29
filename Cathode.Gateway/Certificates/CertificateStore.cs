using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Cathode.Gateway.Certificates
{
    public class CertificateStore : ICertificateStore
    {
        private readonly ILogger<CertificateStore> _logger;
        private readonly ConcurrentDictionary<string, X509Certificate2> _certs;

        public CertificateStore(ILogger<CertificateStore> logger)
        {
            _logger = logger;
            _certs = new ConcurrentDictionary<string, X509Certificate2>();
        }

        public void PruneCertificates()
        {
            foreach (var entry in _certs.Where(entry => entry.Value.NotAfter < DateTime.Now))
            {
                _logger.LogInformation("Removing invalid certificate for {}", entry.Key);
                _certs.TryRemove(entry);
            }
        }

        public IEnumerable<X509Certificate2> GetCertificates()
        {
            return _certs.Values.ToList();
        }

        public X509Certificate2? GetCertificate(string domain)
        {
            return !_certs.TryGetValue(domain, out var cert) ? null : cert;
        }

        public void AddCertificate(X509Certificate2 certificate)
        {
            foreach (var name in certificate.GetAllNames())
            {
                if (_certs.TryAdd(name, certificate))
                {
                    _logger.LogInformation("Loaded certificate for {}", name);
                }
            }
        }

        public void RemoveCertificate(X509Certificate2 certificate)
        {
            foreach (var entry in _certs.Where(entry => entry.Value.Equals(certificate)))
            {
                _certs.TryRemove(entry);
            }
        }

        public X509Certificate2? SelectCertificate(ConnectionContext context, string? domain)
        {
            return domain == null ? null : GetCertificate(domain);
        }
    }
}