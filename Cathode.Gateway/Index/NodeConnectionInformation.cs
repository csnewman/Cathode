using System;

namespace Cathode.Gateway.Index
{
    public class NodeConnectionInformation
    {
        public Guid Id { get; set; }

        public Guid NodeId { get; set; }

        public Node Node { get; set; }

        public string Address { get; set; }

        public int Priority { get; set; }
    }
}