using System.Text.Json;
using LiveMap.Models;

namespace LiveMap.Extensions;

public static class HttpResponseExtensions
{
    public static void SendEvent(this HttpResponse response, string eventName, object data)
    {
        response.Write("event: " + eventName + "\n");
        response.Write("data: " + JsonSerializer.Serialize(data) + "\n\n");
    }
}