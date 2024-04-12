using CitizenFX.Core;
using CitizenFX.Server;
using LiveMap.Shared;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Handlers.Events;

public class PlayerEvents(ILogger<PlayerEvents> logger, PlayerHandler playerHandler, SseHandler sseHandler)
{
    [EventHandler("playerDropped")]
    public void OnPlayerDropped([Source] Player player)
    {
        logger.LogInformation("Player {Player} dropped", player.Name);
        var playerLeftPacket = playerHandler.RemovePlayer(player.Handle);
        sseHandler.BroadcastEvent(Constants.Sse.PlayerLeave, playerLeftPacket);
    }
    
    [EventHandler(Constants.Events.Server.PlayerSpawned, Binding.Remote)]
    public void OnPlayerSpawned([Source] Player player)
    {
        logger.LogInformation("Player {Player} spawned", player.Name);
        var playerSpawnedPacket = playerHandler.AddPlayer(player);
        sseHandler.BroadcastEvent(Constants.Sse.PlayerJoin, playerSpawnedPacket);
    }
    
    
}