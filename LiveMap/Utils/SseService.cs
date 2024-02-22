using LiveMap.Extensions;
using LiveMap.Models;

namespace LiveMap.Utils;

public class SseService
{
    // This is a simple class that will be used to send Server-Sent Events to the client

    private readonly List<HttpResponse> _openResponses = new();

    public void AddResponse(HttpResponse response)
    {
        _openResponses.Add(response);
        response.SendEvent("connected", new { Connected = true });
    }

    public void RemoveResponse(HttpResponse response)
    {
        _openResponses.Remove(response);
    }

    public void BroadcastEvent(string eventName, object data)
    {
        foreach (var response in _openResponses) response.SendEvent(eventName, data);
    }

    public void KeepAlivePing()
    {
        foreach (var response in _openResponses) response.Write(":keep-alive\n\n");
    }
}