using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CitizenFX.Core;

namespace LiveMap.Models
{
    public class HttpResponse
    {
        [JsonPropertyName("writeHead")]
        public CallbackDelegate WriteHeadInternal { get; set; }
        
        [JsonPropertyName("write")]
        public CallbackDelegate WriteInternal { get; set; }
        
        [JsonPropertyName("send")]
        public CallbackDelegate SendInternal { get; set; }
        
        public void WriteHead(int code, IDictionary<string, object> headers)
        {
            WriteHeadInternal?.Invoke(code, headers);
        }
        
        public void WriteHead(int code)
        {
            WriteHeadInternal?.Invoke(code);
        }
        
        public void Write(string data)
        {
            WriteInternal?.Invoke(data);
        }
        
        public void Send(string data = null)
        {
            SendInternal?.Invoke(data);
        }
        
        public void Send<T>(T data)
        {
            SendInternal?.Invoke(JsonSerializer.Serialize(data));
        }
        
    }
}