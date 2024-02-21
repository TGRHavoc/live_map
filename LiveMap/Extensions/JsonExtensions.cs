using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiveMap.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        // This method is used to serialize an object to a JSON string
        public static string ToJson(this object obj, JsonSerializerOptions options = null)
            => JsonSerializer.Serialize(obj, options ?? DefaultOptions);

        // This method is used to deserialize a JSON string to an object
        public static T FromJson<T>(this string json, JsonSerializerOptions options = null)
            => JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
    }
}