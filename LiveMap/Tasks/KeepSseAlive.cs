using CitizenFX.Core;
using LiveMap.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Tasks;

public class KeepSseAlive : ITask
{
    private readonly ILogger<KeepSseAlive> _logger;
    private readonly SseService _sseService;

    public KeepSseAlive(SseService sseService, ILogger<KeepSseAlive> logger)
    {
        _sseService = sseService;
        _logger = logger;
        // Tick += Execute;
    }

    public async Coroutine Execute()
    {
        await Coroutine.Delay(10_000);
        _sseService.KeepAlivePing();
    }
}

public interface ITask
{
    Coroutine Execute();
}