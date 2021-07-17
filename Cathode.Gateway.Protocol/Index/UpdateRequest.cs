using System.ComponentModel.DataAnnotations;

namespace Cathode.Gateway.Protocol.Index
{
    public class UpdateRequest
    {
        public string? ControlToken { get; set; }

        [MinLength(1)]
        public ConnectionInformation[] ConnectionInfo { get; set; }

        public class ConnectionInformation
        {
            public string Address { get; set; }

            public int Priority { get; set; }
        }
    }
}