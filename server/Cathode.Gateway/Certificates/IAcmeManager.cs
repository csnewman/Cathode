namespace Cathode.Gateway.Certificates
{
    public interface IAcmeManager
    {
        void StoreChallenge(string token, string response);

        bool TryGetChallenge(string token, out string? value);
    }
}