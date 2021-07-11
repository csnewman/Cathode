using Cathode.Gateway.Authentication;
using JsonSubTypes;
using Newtonsoft.Json;

namespace Cathode.Gateway
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(AuthenticationSettings), AuthenticationSettings.TypeId)]
    public abstract class GatewaySetting
    {
        public abstract string Type { get; }
    }
}