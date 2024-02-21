using CitizenFX.Core;
using LiveMap.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Tasks
{
    public class KeepSseAlive : ITask
    {
        private readonly SseService _sseService;
        private readonly ILogger<KeepSseAlive> _logger;
        public KeepSseAlive(SseService sseService, ILogger<KeepSseAlive> logger)
        {
            _sseService = sseService;
            _logger = logger;
            // Tick += Execute;
        }

        public async Coroutine Execute()
        {
            await Coroutine.Delay(10_000);
            await _sseService.KeepAlivePing();
        }
    }

    public interface ITask
    {
        Coroutine Execute();
    }
}