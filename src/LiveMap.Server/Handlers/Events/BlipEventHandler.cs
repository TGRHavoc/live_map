using CitizenFX.Core;
using CitizenFX.Server;
using LiveMap.Shared;
using LiveMap.Shared.Models;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Handlers.Events;

public class BlipEventHandler(ILogger<BlipEventHandler> logger, BlipHandler blipHandler, SseHandler sseHandler)
{
    private int? _playerWhoGeneratingBlips = null;

    [EventHandler(Constants.Events.Internal.BlipsRequested, Binding.Local)]
    public void OnBlipsRequested(int sourceId)
    {
        logger.LogInformation("Player {Player} requested blips", sourceId);
        _playerWhoGeneratingBlips = sourceId;
        // _sseService.AddResponse(player.Response);
        //_sseService.BroadcastEvent<EmptyMessage>(Constants.Sse.RefreshBlips);
    }
    
    [EventHandler(Constants.Events.Server.GeneratedBlips, Binding.Remote)]
    public void OnBlipsGenerated([Source] Player player, Dictionary<int, List<Blip>> obj)
    {
        if (player.Handle != _playerWhoGeneratingBlips)
        {
            logger.LogWarning(
                "Player {Player} tried to send blips but they are not the player who generated them, ignoring...",
                player.Name);
            return;
        }
        
        logger.LogInformation("Received {BlipCount} blips from player {Player}", blipHandler.BlipCount, player.Name);

        blipHandler.SetBlips(obj);
        _playerWhoGeneratingBlips = null;
        blipHandler.SaveBlips();
        sseHandler.BroadcastEvent<EmptyMessage>(Constants.Sse.RefreshBlips);
    }
    
}