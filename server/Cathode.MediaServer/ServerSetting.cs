using System;
using JsonSubTypes;
using Newtonsoft.Json;

namespace Cathode.MediaServer
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    public abstract class ServerSetting : ICloneable
    {
        public abstract string Type { get; }

        public abstract object Clone();
    }
}