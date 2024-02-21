using System.Dynamic;
using LiveMap.Extensions;
using LiveMap.Models;
using LiveMap.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Handlers
{
    public class BasicHttpHandler
    {
        private readonly BlipHandler _blipHandler;
        private readonly ILogger<BasicHttpHandler> _logger;
        private readonly SseService _sseService;

        public BasicHttpHandler(BlipHandler blipHandler, ILogger<BasicHttpHandler> logger, SseService sseService)
        {
            _logger = logger;
            _sseService = sseService;
            _blipHandler = blipHandler;
            _logger.LogDebug("BasicHttpHandler created");
        }

        public void OnHttpEndpointRequest(ExpandoObject request, ExpandoObject response)
        {
            var req = request.FromExpando<HttpRequest>();
            var res = response.FromExpando<HttpResponse>();

            if (res == null)
            {
                _logger.LogWarning("Response object is null");
                return;
            }

            _logger.LogDebug("Received HTTP request for {Path}", req.Path);

            var accessControlValue = Config.GetConfigKeyValue(Constants.Config.AccessControlOrigin, 0, "*", _logger);

            var headers = new Dictionary<string, object>
            {
                { "Access-Control-Allow-Origin", accessControlValue },
                { "Access-Control-Allow-Methods", "GET, OPTIONS" },
                // We don't accept any headers
                { "Access-Control-Allow-Headers", "" },
                // We only respond with JSON
                { "Content-Type", "application/json" },
            };

            if (req.Path == "/sse")
            {
                req.SetCancelHandler(() =>
                {
                    _logger.LogDebug("Request was cancelled?");
                    // Debug.WriteLine("Request was cancelled?");
                    // _openResponses.Remove(res);
                    _sseService.RemoveResponse(res);
                });
                
                req.SetDataHandler(new Action<string>(str =>
                {
                    _logger.LogInformation("Received data: {Data}", str);
                }));


                // Change content type to text/event-stream
                headers["Content-Type"] = "text/event-stream";
                // Keep the connection open
                headers["Connection"] = "keep-alive";

                // Server-Sent Events
                res.WriteHead(200, new Dictionary<string, object>
                {
                    { "Content-Type", "text/event-stream" },
                    { "Connection", "keep-alive" },
                    { "Cache-Control", "no-cache" },
                    { "Access-Control-Allow-Origin", accessControlValue },
                });

                _sseService.AddResponse(res);
                _sseService.BroadcastEvent("newClient", new
                {
                    Client = req.Address
                });
                
                return;
                //res.Write("data: Hello, world!\n\n");
                // _openResponses.Add(res);
            }
            else if (req.Path == "/blips" || req.Path == "/blips.json")
            {
                res.WriteHead(200, headers);
                _blipHandler.SendBlips(res);
                //res.WriteHead(200);
                //res.Send(_blipHandler.Blips);
            }

            res.WriteHead(404, headers);
            res.Send(new
            {
                Error = $"path {req.Path} not found"
            });

            //res.WriteHead(200);

            // Debug.WriteLine("Received HTTP request!");
            //res.WriteHead(200, new Dictionary<string, object>());
            // res.Send(JsonSerializer.Serialize(new
            // {
            //     pong=true
            // }));
        }
    }
}