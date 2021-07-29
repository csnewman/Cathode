using System.Threading.Tasks;

namespace Cathode.Gateway.Certificates
{
    public interface IAcmeProcessor
    {
        Task LoadCertificateAsync();

        Task CheckCertificateAsync();
    }
}