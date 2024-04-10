using System.Text.Json;
using CitizenFX.Server.Native;
using LiveMap.Server.Utils;
using LiveMap.Shared.Models;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Handlers;

public class BlipHandler : IDisposable
{
    private readonly string _blipFile;

    private readonly ILogger<BlipHandler> _logger;

    public BlipHandler(ILogger<BlipHandler> logger)
    {
        _logger = logger;
        _blipFile = Config.GetConvarValue(Constants.Config.BlipFile, Constants.DefaultBlipFile);

        LoadBlips();
    }

    public int BlipCount => Blips.Aggregate(0, (current, blip) => current + blip.Value.Count);

    public Dictionary<int, List<Blip>> Blips { get; private set; } = new();

    public void LoadBlips()
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
    
    public void SetBlips(Dictionary<int, List<Blip>> blips)
    {
        Blips = blips;
    }

    public bool ContainsBlip(int spriteId, MapVec3 pos)
    {
        return Blips.ContainsKey(spriteId) && Blips[spriteId].Exists(b => b.Pos.Equals(pos));
    }

    public bool ValidateClientSentBlip(Blip blip)
    {
        if (blip.Sprite == null) return false;
        if (blip.Pos.Equals(null)) return false;
        return blip.Pos.X != 0.0 || blip.Pos.Y != 0.0 || blip.Pos.Z != 0.0;
    }

    public void Dispose()
    {
        _logger.LogDebug("Disposing BlipHandler...");
        SaveBlips();
    }
}