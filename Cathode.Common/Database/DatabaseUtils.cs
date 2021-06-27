using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Npgsql;

namespace Cathode.Common.Database
{
    public static class DatabaseUtils
    {
        public static void ConfigureJson()
        {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(new CamelCaseNamingStrategy())
                }
            });
        }
    }
}