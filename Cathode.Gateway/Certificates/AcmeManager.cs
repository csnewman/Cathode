namespace Cathode.Gateway.Certificates
{
    public class AcmeManager : IAcmeManager
    {
        private (string, string)? _certChallenge;

        public void StoreChallenge(string token, string response)
        {
            _certChallenge = (token, response);
        }

        public bool TryGetChallenge(string token, out string? value)
        {
            if (!_certChallenge.HasValue || _certChallenge.Value.Item1 != token)
            {
                value = null;
                return false;
            }

            value = _certChallenge.Value.Item2;
            return true;
        }
    }
}