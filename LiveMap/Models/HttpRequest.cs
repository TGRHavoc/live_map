using System.Text.Json.Serialization;
using CitizenFX.Core;

namespace LiveMap.Models;

public class HttpRequest
{
    [JsonPropertyName("address")] public string Address { get; set; } = null!;

    [JsonPropertyName("method")] public string Method { get; set; } = null!;

    [JsonPropertyName("path")] public string Path { get; set; } = null!;

    [JsonPropertyName("headers")] public IDictionary<string, string> Headers { get; set; } = null!;

    [JsonPropertyName("setDataHandler")] public Callback? SetDataHandlerInternal { get; set; }

    [JsonPropertyName("setCancelHandler")] public Callback? SetCancelHandlerInternal { get; set; }

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