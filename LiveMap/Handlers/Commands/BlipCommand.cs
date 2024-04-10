using CitizenFX.Core;
using CitizenFX.Server;
using LiveMap.Extensions;
using LiveMap.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Handlers.Commands;

public class BlipCommand(ILogger<BlipCommand> logger, BlipHandler blipHandler, SseHandler sseHandler) : BaseScript
{
    [Command("blips", Restricted = true, RemapParameters = true)]
    public void BlipsCommand([Source] Player source, object[] args)
    {
        logger.LogInformation("Blips command called! {Args} by {Source}", string.Join(",", args), source.ToJson());
        if (source.Handle != 0)
        {
            // they can only use the "generate" command and nothing else
            if (args.Length == 0 || args[0].ToString().ToLower() != "generate")
            {
                logger.LogWarning("Player {Id} tried to use the blips command with invalid arguments '{Args}'",
                    source.Name, string.Join(",", args));
                return;
            }
            
            logger.LogInformation("Player {Player} is generating blips", source.Name);
            CitizenFX.Core.Events.TriggerEvent(Constants.Events.Internal.BlipsRequested, source.Handle);
            source.TriggerEvent(Constants.Events.Client.GenerateBlips);
            return;
        }

        if (args.Length == 0)
        {
            // Count of blips
            logger.LogInformation("There are {BlipCount} blips in the cache", blipHandler.BlipCount);
            logger.LogInformation("If you want to save the blips to file, use 'blips save'");
            logger.LogInformation("If you want to reload the blips from file, use 'blips reload'");
            return;
        }

        switch (args[0].ToString().ToLower())
        {
            // If the first argument is "save", save the blips to file
            case "save":
            {
                blipHandler.SaveBlips();
                logger.LogInformation("Saved {BlipCount} blips to file", blipHandler.BlipCount);
                break;
            }
            // If the first argument is "reload" then we reload the blips from file
            case "reload":
            {
                logger.LogInformation("Reloading blips from file... Any unsaved changes will be lost");
                blipHandler.LoadBlips();

                sseHandler.BroadcastEvent<EmptyMessage>(Constants.Sse.RefreshBlips);

                break;
            }
            case "generate":
            {
                logger.LogInformation("Generating blips..."); 
                CitizenFX.Core.Events.TriggerEvent("livemap:__blipsRequested", source.Handle);
                break;
            }
            default:
                // Help message
                logger.LogInformation("There are {BlipCount} blips in the cache", blipHandler.BlipCount);
                logger.LogInformation("If you want to save the blips to file, use 'blips save'");
                logger.LogInformation("If you want to reload the blips from file, use 'blips reload'");
                break;
        }
    }

    // [Command("blips", true)]
    // public void BlipsCommandFromPlayer([Source] Player source, object[] args)
    // {
    //     // Since this has come from an actual player, they can only use the "generate" command and nothing else
    //     if (args.Length == 0 || args[0].ToString().ToLower() != "generate")
    //     {
    //         logger.LogWarning("Player {Id} tried to use the blips command with invalid arguments '{Args}'",
    //             source.Name, string.Join(",", args));
    //         return;
    //     }
    //
    //     logger.LogInformation("Player {Player} is generating blips", source.Name);
    //     //_playerWhoGeneratingBlips = source;
    //     CitizenFX.Core.Events.TriggerEvent("livemap:__blipsRequested", source);
    //
    //     source.TriggerEvent("livemap:generateBlips");
    // }
}