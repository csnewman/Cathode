using System;

namespace Cathode.Gateway.Protocol.Index
{
    public class RegisterRequest
    {
        public string AccountId { get; set; }

        public Guid DeviceId { get; set; }

        public string LookupToken { get; set; }
    }
}