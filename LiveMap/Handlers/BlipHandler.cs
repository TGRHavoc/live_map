using System.Text.Json;
using CitizenFX.Core;
using CitizenFX.Server;
using CitizenFX.Server.Native;
using LiveMap.Models;
using LiveMap.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Handlers;

public class BlipHandler
{
    private readonly string _blipFile;

    private readonly ILogger<BlipHandler> _logger;
    private readonly SseService _sseService;

    private Player? _playerWhoGeneratingBlips;

    public BlipHandler(ILogger<BlipHandler> logger, SseService sseService)
    {
        _logger = logger;
        _sseService = sseService;
        _blipFile = Config.GetConfigKeyValue(Constants.Config.BlipFile, 0, Constants.DefaultBlipFile, logger);

        LoadBlips();
    }

    public int BlipCount => Blips.Aggregate(0, (current, blip) => current + blip.Value.Count);

    public Dictionary<int, List<Blip>> Blips { get; private set; } = new();

    private void LoadBlips()
    {
        // Load blips from file, if it exists
        var fileData = Natives.LoadResourceFile(LiveMap.ResourceName, _blipFile);

        if (string.IsNullOrEmpty(fileData))
        {
            _logger.LogInformation("Blip file '{BlipFile}' does not exist or is empty", _blipFile);
        }
        else
        {
            Blips = JsonSerializer.Deserialize<Dictionary<int, List<Blip>>>(fileData) ??
                    new Dictionary<int, List<Blip>>();
            _logger.LogInformation("Loaded {BlipCount} blips from file", BlipCount);
        }
    }

    public void BlipsCommand(int source, object[] args)
    {
        _logger.LogInformation("Blips command called! {Args} by {Source}", string.Join(",", args), source);
        if (source != 0) return;

        if (args.Length == 0)
        {
            // Count of blips
            _logger.LogInformation("There are {BlipCount} blips in the cache", BlipCount);
            _logger.LogInformation("If you want to save the blips to file, use 'blips save'");
            _logger.LogInformation("If you want to reload the blips from file, use 'blips reload'");
            return;
        }

        switch (args[0].ToString().ToLower())
        {
            // If the first argument is "save", save the blips to file
            case "save":
            {
                SaveBlips();
                _logger.LogInformation("Saved {BlipCount} blips to file", BlipCount);
                break;
            }
            // If the first argument is "reload" then we reload the blips from file
            case "reload":
            {
                _logger.LogInformation("Reloading blips from file... Any unsaved changes will be lost");
                LoadBlips();
                
                _sseService.BroadcastEvent(Constants.Sse.RefreshBlips);

                break;
            }
            default:
                // Help message
                _logger.LogInformation("There are {BlipCount} blips in the cache", BlipCount);
                _logger.LogInformation("If you want to save the blips to file, use 'blips save'");
                _logger.LogInformation("If you want to reload the blips from file, use 'blips reload'");
                break;
        }
    }

    public void BlipsCommandFromPlayer(Player source, object[] args)
    {
        // Since this has come from an actual player, they can only use the "generate" command and nothing else
        if (args.Length == 0 || args[0].ToString().ToLower() != "generate")
        {
            _logger.LogWarning("Player {Id} tried to use the blips command with invalid arguments '{Args}'",
                source.Name, string.Join(",", args));
            return;
        }

        _logger.LogInformation("Player {Player} is generating blips", source.Name);
        _playerWhoGeneratingBlips = source;
        source.TriggerEvent("livemap:generateBlips");
    }

    public void SaveBlips()
    {
        if (BlipCount == 0)
        {
            _logger.LogInformation("No blips to save");
            return;
        }

        var fileData = JsonSerializer.Serialize(Blips, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var saved = Natives.SaveResourceFile(LiveMap.ResourceName, _blipFile, fileData, fileData.Length);
        if (!saved)
            _logger.LogError("Failed to save blips to file '{BlipFile}', could be a permission error if on Linux",
                _blipFile);
    }

    public bool ContainsBlip(int spriteId, Position pos)
    {
        return Blips.ContainsKey(spriteId) && Blips[spriteId].Exists(b => b.Pos.Equals(pos));
    }

    public bool ValidateClientSentBlip(Blip blip)
    {
        if (blip.Sprite == null) return false;
        if (blip.Pos.Equals(null)) return false;
        return blip.Pos.X != 0.0 || blip.Pos.Y != 0.0 || blip.Pos.Z != 0.0;
    }

    [EventHandler("livemap:AddBlip")]
    public void AddBlip([Source] Player player, Blip blip)
    {
        if (!ValidateClientSentBlip(blip))
        {
            _logger.LogWarning("Player {Player} tried to add an invalid blip", player.Name);
            return;
        }

        if (Blips.ContainsKey(blip.Sprite!.Value))
            Blips[blip.Sprite.Value].Add(blip);
        else
            Blips[blip.Sprite.Value] = new List<Blip> { blip };

        _logger.LogInformation("Player {Player} added a new blip {Blip}", player.Name,
            JsonSerializer.Serialize(blip));
        _sseService.BroadcastEvent(Constants.Sse.AddBlip, blip);
    }

    [EventHandler("livemap:blipsGenerated", Binding.Remote)]
    public void OnBlipsGenerated([Source] Player player, Dictionary<int, List<Blip>> obj)
    {
        if (player != _playerWhoGeneratingBlips)
        {
            _logger.LogWarning(
                "Player {Player} tried to send blips but they are not the player who generated them, ignoring...",
                player.Name);
            return;
        }

        Blips = obj;
        _logger.LogInformation("Received {BlipCount} blips from player {Player}", BlipCount, player.Name);
        SaveBlips();
        
        _sseService.BroadcastEvent(Constants.Sse.RefreshBlips);
    }

    [EventHandler("livemap:UpdateBlip")]
    public void UpdateBlip(Player player, Blip blip)
    {
        if (blip.Sprite == null)
        {
            _logger.LogWarning("Player {Player} tried to update a blip with no sprite", player.Name);
            return;
        }

        // We want to get the player's position and get the closest blip to that position and update it with the new blip's data...
        var closestBlip = Blips[blip.Sprite!.Value].OrderBy(b => b.Pos.DistanceToSquared(blip.Pos)).First();
        closestBlip.Name = blip.Name;
        closestBlip.Description = blip.Description;
        
        _logger.LogInformation("Player {Player} updated blip {Blip}", player.Name, JsonSerializer.Serialize(blip));
        _sseService.BroadcastEvent(Constants.Sse.UpdateBlip, closestBlip);
    }
}