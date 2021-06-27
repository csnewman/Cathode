using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Cathode.Common
{
    public static class ConfigurationUtils
    {
        public static string? ParseString(this IConfiguration configuration, string? defaultValue, params string[] key)
        {
            IConfigurationSection? section = null;
            for (var i = 0; i < key.Length; i++)
            {
                var part = key[i];

                if (i == key.Length - 1)
                {
                    var value = section == null ? configuration[part] : section[part];
                    if (value != null)
                    {
                        return value;
                    }
                }
                else
                {
                    section = section == null ? configuration.GetSection(part) : section.GetSection(part);
                    if (section == null)
                    {
                        break;
                    }
                }
            }

            var globalKey = string.Join("_", key.Select(x => x.ToUpper()));
            return configuration[globalKey] ?? defaultValue;
        }

        public static bool ParseBool(this IConfiguration configuration, bool defaultValue, params string[] key)
        {
            var value = configuration.ParseString(null, key);
            return value != null ? bool.Parse(value) : defaultValue;
        }

        public static T ParseEnum<T>(this IConfiguration configuration, T defaultValue, params string[] key)
            where T : struct
        {
            var value = configuration.ParseString(null, key);
            return value != null ? Enum.Parse<T>(value, true) : defaultValue;
        }
    }
}