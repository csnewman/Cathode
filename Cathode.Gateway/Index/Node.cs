using System;
using System.Collections.Generic;

namespace Cathode.Gateway.Index
{
    public class Node
    {
        public Guid Id { get; set; }

        public string AccountId { get; set; }

        public Guid DeviceId { get; set; }

        public string LookupToken { get; set; }

        public string AuthenticationToken { get; set; }

        public Guid ControlTokenChallenge { get; set; }

        public string? ControlToken { get; set; }

        public DateTime FirstSeen { get; set; }

        public DateTime LastSeen { get; set; }

        public ICollection<NodeConnectionInformation> ConnectionInfo { get; set; }

        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(ControlToken);
        }
    }
}