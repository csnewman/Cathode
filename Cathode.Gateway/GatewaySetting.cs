using JsonSubTypes;
using Newtonsoft.Json;

namespace Cathode.Gateway
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    public abstract class GatewaySetting
    {
        public abstract string Type { get; }
    }
}