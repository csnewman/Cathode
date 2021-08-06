using System;

namespace Cathode.Gateway.Protocol.Index
{
    public class LookupResponse
    {
        public DeviceInformation[] Devices { get; set; }

        public class DeviceInformation
        {
            public Guid RegistrationId { get; set; }

            public Guid DeviceId { get; set; }

            public Guid ControlChallenge { get; set; }

            public DateTime FirstSeen { get; set; }

            public DateTime LastSeen { get; set; }

            public ConnectionInformation[] ConnectionInfo { get; set; }
        }
    }
}