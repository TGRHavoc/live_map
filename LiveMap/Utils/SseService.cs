using Google.Protobuf;
using LiveMap.Extensions;
using LiveMap.Models;

namespace LiveMap.Utils;

public class SseService
{
    // This is a simple class that will be used to send Server-Sent Events to the client

    private readonly List<HttpResponse> _openResponses = new();

    public SseService()
    {
        // Disable GC for this class as we need to keep the open responses in memory
        GC.SuppressFinalize(this);
    }

    public void AddResponse(HttpResponse response)
    {
        _openResponses.Add(response);

        response.Write("retry: 10000\n\n");
        response.SendEvent("connected", new ConnectedMessage { Connected = true });
    }

    public void RemoveResponse(HttpResponse response)
    {
        _openResponses.Remove(response);
    }

    public void BroadcastEvent<T>(string eventName, T? data = null)
        where T : class, IMessage<T>, new()
    {
        foreach (var response in _openResponses) response.SendEvent(eventName, data);
    }

    public void BroadcastKeepAlive()
    {
        foreach (var response in _openResponses) response.Write(":keep-alive\n\n");
    }
}