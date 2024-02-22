using System.Dynamic;
using System.Text.Json.Serialization;
using CitizenFX.Core;

namespace LiveMap.Models;

public class HttpRequest
{
    [JsonPropertyName("address")] public string Address { get; set; } = null!;

    [JsonPropertyName("method")] public string Method { get; set; } = null!;

    [JsonPropertyName("path")] public string Path { get; set; } = null!;

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("headers")] public ExpandoObject HeadersInternal { get; set; } = null!;

    public IDictionary<string, object> Headers => new Dictionary<string, object>(HeadersInternal);

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