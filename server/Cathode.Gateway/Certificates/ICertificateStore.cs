using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;

namespace Cathode.Gateway.Certificates
{
    public interface ICertificateStore
    {
        void PruneCertificates();

        IEnumerable<X509Certificate2> GetCertificates();

        X509Certificate2? GetCertificate(string domain);

        void AddCertificate(X509Certificate2 certificate);

        void RemoveCertificate(X509Certificate2 certificate);

        X509Certificate2? SelectCertificate(ConnectionContext context, string? domain);
    }
}