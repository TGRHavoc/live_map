using Google.Protobuf;
using LiveMap.Models;
using Microsoft.Extensions.Logging;

namespace LiveMap.Extensions;

public static class HttpResponseExtensions
{
    public static void SendEvent<T>(this HttpResponse response, string eventName, T? data = null,
        ILogger? logger = null)
        where T : class, IMessage<T>
    {
        logger?.LogTrace("Sending event: {EventName} with data: {Data}", eventName, data.ToByteString().ToBase64());

        response.Write("event: " + eventName + "\n");
        response.Write("data: " + data.ToByteString().ToBase64() + "\n\n");
    }
}