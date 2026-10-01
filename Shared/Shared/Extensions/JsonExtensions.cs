using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace Vero.Shared.Extensions
{
    public static class JsonConfig
    {
        public static DefaultContractResolver ContractResolver() => new() { NamingStrategy = new CamelCaseNamingStrategy(), };

        private static readonly JsonSerializerSettings _serializerSettings;

        static JsonConfig()
        {
            _serializerSettings = new()
            {
                ContractResolver = ContractResolver(),
                Formatting = Formatting.Indented,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };

            _serializerSettings.Converters.Add(new IsoDateTimeConverter
            {
                DateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal
            });
        }

        public static JsonSerializerSettings SerializerSettings() => _serializerSettings;

        public static string Serialize(object o) => JsonConvert.SerializeObject(o, SerializerSettings());

        public static T? Deserialize<T>(string s) => JsonConvert.DeserializeObject<T>(s);

        public static object? Deserialize(string s) => JsonConvert.DeserializeObject(s);

        public static bool TryParseJson<T>(this string? obj, out T? result)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                result = default;
                return false;
            }

            try
            {
                result = JsonConvert.DeserializeObject<T>(obj);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }
    }
}