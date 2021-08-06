using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cathode.Common.Api
{
    public class ApiContractResolver : DefaultContractResolver
    {
        public static ApiContractResolver Instance { get; }

        static ApiContractResolver()
        {
            Instance = new ApiContractResolver();
        }

        private ApiContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.OrderBy(p => BaseTypesAndSelf(p.DeclaringType).Count()).ToList();

            static IEnumerable<Type> BaseTypesAndSelf(Type? type)
            {
                while (type != null)
                {
                    yield return type;
                    type = type.BaseType;
                }
            }
        }
    }
}