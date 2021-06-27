using System.ComponentModel.DataAnnotations;

namespace Cathode.Gateway.Protocol.Index
{
    public class IndexRegisterRequest
    {
        public string AccountId { get; set; }

        public string DeviceId { get; set; }

        [MinLength(1)]
        public ConnectionInformation[] ConnectionInfo { get; set; }

        public class ConnectionInformation
        {
            public string Address { get; set; }

            public int Priority { get; set; }
        }
    }
}