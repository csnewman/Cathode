using System;
using JsonSubTypes;
using Newtonsoft.Json;

namespace Cathode.Gateway
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    public abstract class GatewaySetting
    public abstract class GatewaySetting : ICloneable
    {
        public abstract string Type { get; }

        public abstract object Clone();
    }
}