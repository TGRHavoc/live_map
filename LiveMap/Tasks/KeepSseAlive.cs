using CitizenFX.Core;
using LiveMap.Handlers;

namespace LiveMap.Tasks;

public class KeepSseAlive(SseHandler sseHandler) : ITask
{
    public async Coroutine Execute()
    {
        await Coroutine.Delay(10_000);
        sseHandler.BroadcastKeepAlive();
    }
}