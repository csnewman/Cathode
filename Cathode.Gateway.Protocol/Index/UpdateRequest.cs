using System.ComponentModel.DataAnnotations;

namespace Cathode.Gateway.Protocol.Index
{
    public class UpdateRequest
    {
        public string? ControlToken { get; set; }

        public string? AcmeChallenge { get; set; }

        [MinLength(1)]
        public ConnectionInformation[] ConnectionInfo { get; set; }
    }
}