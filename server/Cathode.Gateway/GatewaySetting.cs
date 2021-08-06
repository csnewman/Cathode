using System;
using Cathode.Gateway.Certificates;
using JsonSubTypes;
using Newtonsoft.Json;

namespace Cathode.Gateway
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(AcmeSettings), AcmeSettings.Id)]
    public abstract class GatewaySetting : ICloneable
    {
        public abstract string Type { get; }

        public abstract object Clone();
    }
}