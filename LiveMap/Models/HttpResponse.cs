using System.Text.Json;
using System.Text.Json.Serialization;
using CitizenFX.Core;

namespace LiveMap.Models;

public struct HttpResponse
{
    [JsonPropertyName("writeHead")] public Callback? WriteHeadInternal { get; set; }

    [JsonPropertyName("write")] public Callback? WriteInternal { get; set; }

    [JsonPropertyName("send")] public Callback? SendInternal { get; set; }

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

    public void Send(string? data = null)
    {
        SendInternal?.Invoke(data);
    }

    public void Send<T>(T data)
    {
        SendInternal?.Invoke(JsonSerializer.Serialize(data));
    }
}