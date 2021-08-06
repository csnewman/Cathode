using System;

namespace Cathode.Gateway.Protocol.Index
{
    public class RegisterResponse
    {
        public Guid RegistrationId { get; set; }

        public string AuthenticationToken { get; set; }

        public Guid ControlTokenSalt { get; set; }
    }
}