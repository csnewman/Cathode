using System;

namespace Cathode.Gateway.Index
{
    public class NodeConnectionInformation
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public Node Node { get; set; }

        public string Address { get; set; }

        public int Priority { get; set; }

        public DateTime LastSeen { get; set; }
    }
}