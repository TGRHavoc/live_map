using System.Dynamic;
using LiveMap.Server.Extensions;
using LiveMap.Server.Models;
using LiveMap.Server.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Handlers;

public class BasicHttpHandler
{
    private readonly BlipHandler _blipHandler;
    private readonly ILogger<BasicHttpHandler> _logger;
    private readonly SseHandler _sseHandler;
    private readonly PlayerHandler _playerHandler;
    
    private delegate void EndpointHandler(HttpRequest request, HttpResponse response, Dictionary<string, object> headers);
    private readonly Dictionary<string, EndpointHandler> _endpoints = new();

    public BasicHttpHandler(BlipHandler blipHandler, ILogger<BasicHttpHandler> logger, SseHandler sseHandler,
        PlayerHandler playerHandler)
    {
        _logger = logger;
        _sseHandler = sseHandler;
        _blipHandler = blipHandler;
        _playerHandler = playerHandler;
        _logger.LogDebug("BasicHttpHandler created");
        
        _endpoints.Add("/sse", OnSseRequest);
        _endpoints.Add("/blips", OnBlipsRequest);
        _endpoints.Add("/blips.json", OnBlipsRequest);
    }

    private void OnBlipsRequest(HttpRequest req, HttpResponse res, Dictionary<string, object> headers)
    {
        res.WriteHead(200, headers);
        res.SendJson(_blipHandler.Blips);
    }

    private void OnSseRequest(HttpRequest req, HttpResponse res, Dictionary<string, object> headers)
    {
        req.SetCancelHandler(() =>
        {
            _logger.LogDebug("Request was cancelled?");
            // Debug.WriteLine("Request was cancelled?");
            // _openResponses.Remove(res);
            _sseHandler.RemoveResponse(res);
        });
        
        // Keep the connection open
        //headers["Connection"] = "keep-alive";
        // Change content type to text/event-stream
        headers["Content-Type"] = "text/event-stream";
        headers["Cache-Control"] = "no-cache";
        headers["X-Accel-Buffering"] = "no";
        headers["Transfer-Encoding"] = "chunked";
        
        _logger.LogInformation("New SSE connection from {Address}", req.Address);
        
        // Server-Sent Events
        res.WriteHead(200, headers);
        
        _sseHandler.AddResponse(res);
        
        // // Send the current players to the new connection so they
        // // can be displayed on the map
        var players = _playerHandler.GetAllPlayers();
        foreach (var player in players)
        {
            var data = _playerHandler.GetPlayerData(player);
            res.SendEvent(Constants.Sse.PlayerJoin, new PlayerUpdate
            {
                Id = player,
                Data = data
            });
        }
        
    }
    
    public void OnHttpEndpointRequest(ExpandoObject request, ExpandoObject response)
    {
        var req = request.FromExpando<HttpRequest>();
        var res = response.FromExpando<HttpResponse>();

        _logger.LogDebug("Received HTTP request for {Path}", req.Path);

        var accessControlValue = Config.GetConvarValue(Constants.Config.AccessControlOrigin, "*");

        // If we're not allowing _all_ then we need to do some checks
        if (accessControlValue != "*")
        {
            // Make sure we have an origin header. If not, we can't allow it since we can't verify where it's coming from.
            // Note: People _can_ spoof this header, but it's better than nothing, I guess?
            if (!req.Headers.TryGetValue("Origin", out var origin))
            {
                _logger.LogWarning("Request from {Address} does not have an Origin header", req.Address);
                res.WriteHead(400, new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                });
                res.SendJson(new
                {
                    Error = "Unauthorized"
                });
                return;
            }

            // What are we allowing to connect? This can be a comma-separated list of origins
            var allowedOrigins = accessControlValue.Split(',').Select(x => x.Trim());

            // We need to check if the origin is allowed
            if (!allowedOrigins.Contains(origin))
            {
                _logger.LogWarning("Request from {Address} with Origin {Origin} is not allowed", req.Address, origin);
                res.WriteHead(400, new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                });
                res.SendJson(new
                {
                    Error = "Unauthorized"
                });
                return;
            }

            // Set accessControlValue to the origin that was allowed. This is so we can echo it back in the headers.
            accessControlValue = origin.ToString();
        }

        var headers = new Dictionary<string, object>
        {
            { "Access-Control-Allow-Origin", accessControlValue },
            { "Access-Control-Allow-Methods", "GET, OPTIONS" },
            // We don't accept any headers
            { "Access-Control-Allow-Headers", "" },
            // We only respond with JSON. Except for the SSE endpoint, which is text/event-stream but we handle that later
            { "Content-Type", "application/json" }
        };
        
        // If we get an OPTIONS request, we just return the headers
        if (req.Method == "OPTIONS")
        {
            res.WriteHead(200, headers);
            return;
        }
        
        // Check if we have an endpoint for the path
        if (_endpoints.TryGetValue(req.Path, out var endpoint))
        {
            endpoint.Invoke(req, res, headers);
            return;
        }
        
        // If we don't have an endpoint, we return a 404
        
        res.WriteHead(404, headers);
        res.SendJson(new
        {
            Error = $"path {req.Path} not found"
        });
    }
}