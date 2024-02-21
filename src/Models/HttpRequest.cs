using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CitizenFX.Core;

namespace LiveMap.Models
{
    public class HttpRequest
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [JsonPropertyName("method")]
        public string Method { get; set; }
        
        [JsonPropertyName("path")]
        public string Path { get; set; }
        
        [JsonPropertyName("headers")]
        public IDictionary<string, string> Headers { get; set; }
        
        [JsonPropertyName("setDataHandler")]
        public CallbackDelegate SetDataHandlerInternal { get; set; }
        
        [JsonPropertyName("setCancelHandler")]
        public CallbackDelegate SetCancelHandlerInternal { get; set; }
        
        public void SetDataHandler(Action<string> handler)
        {
            SetDataHandlerInternal?.Invoke(handler);
        }
        
        public void SetDataHandler(Action<byte[]> handler)
        {
            SetDataHandlerInternal?.Invoke(handler, "binary");
        }
        
        public void SetCancelHandler(Action handler)
        {
            SetCancelHandlerInternal?.Invoke(handler);
        }
        
    }
}