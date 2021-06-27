using System;
using System.Collections.Generic;

namespace Cathode.Gateway.Index
{
    public class Node
    {
        public int Id { get; set; }

        public string AccountId { get; set; }

        public string DeviceId { get; set; }

        public DateTime FirstSeen { get; set; }

        public DateTime LastSeen { get; set; }

        public ICollection<NodeConnectionInformation> ConnectionInfo { get; set; }
    }
}