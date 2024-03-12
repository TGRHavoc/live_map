using System.Collections.Concurrent;
using CitizenFX.Server;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using LiveMap.Models;
using Microsoft.Extensions.Logging;

namespace LiveMap.Handlers;

public class PlayerHandler
{
    private readonly ConcurrentDictionary<int, PlayerData> _players = new();

    private readonly ILogger<PlayerHandler> _logger;
    //private readonly SseService _sseService;

    public int PlayerCount => _players.Count;

    public int GetNextPlayerHandle()
    {
        var handle = 0;
        while (_players.ContainsKey(handle))
        {
            handle++;
        }

        return handle;
    }

    public PlayerHandler(ILogger<PlayerHandler> logger)
    {
        _logger = logger;
    }

    private void UpdatePlayerIdentifiers(int playerHandle, IDictionary<string, string> identifiers)
    {
        var playerData = _players.GetOrAdd(playerHandle, new PlayerData());

        playerData.Identifiers.Clear();
        playerData.Identifiers.Add(identifiers);
    }

    private void UpdatePlayerDataInternal<T>(int playerHandle, string key, T value)
        where T : class, IMessage<T>
    {
        if (key == "identifiers")
            throw new ArgumentException("Use UpdatePlayerIdentifiers to update identifiers");

        var playerData = _players.GetOrAdd(playerHandle, new PlayerData());

        switch (key)
        {
            case "name" when typeof(T) == typeof(StringValue):
                playerData.Name = (value as StringValue)?.Value;
                return;
            case "position" when typeof(T) == typeof(Vec3):
                playerData.Position = value as Vec3;
                return;
        }

        playerData.Metadata[key] = value as Value;
    }

    public PlayerData GetPlayerData(int playerHandle)
    {
        return _players.TryGetValue(playerHandle, out var playerData) ? playerData : new PlayerData();
    }

    private static Value ValueFromObject(object o)
    {
        Value val = o switch
        {
            string s => Value.ForString(s),
            int i => Value.ForNumber(i),
            float f => Value.ForNumber(f),
            double d => Value.ForNumber(d),
            bool b => Value.ForBool(b),
            _ => Value.ForString(o.ToString())
        };
        return val;
    }

    public PlayerUpdate AddPlayer(Player player)
    {
        // Get all the data and call the other AddPlayer method
        var playerIdentifier = player.Identifiers.ToDictionary(x => x.Split(':')[0], x => x.Split(':')[1]);
        var playerName = player.Name;
        var playerHandle = player.Handle;
        var playerPos = new MapVec3(player.Character.Position);

        return AddPlayer(playerHandle, playerName, playerPos, playerIdentifier);
    }

    public PlayerUpdate AddPlayer(int playerHandle, string playerName, MapVec3 initalPos,
        Dictionary<string, string> identifier,
        IDictionary<string, object>? additionalMeta = null)
    {
        UpdatePlayerDataInternal(playerHandle, "name", new StringValue { Value = playerName });
        UpdatePlayerIdentifiers(playerHandle, identifier);
        UpdatePlayerDataInternal(playerHandle, "position", initalPos.ToVec3());
        
        if (additionalMeta != null)
        {
            foreach (var kvp in additionalMeta)
            {
                UpdatePlayerDataInternal(playerHandle, kvp.Key, ValueFromObject(kvp.Value));
            }
        }

        var update = new PlayerUpdate
        {
            Id = playerHandle,
            Data = _players[playerHandle]
        };

        return update;
    }

    public PlayerPosition MovePlayer(Player player) =>
        MovePlayer(player.Handle, new MapVec3(player.Character.Position));

    public PlayerPosition MovePlayer(int playerHandle, MapVec3 newPos)
    {
        UpdatePlayerDataInternal(playerHandle, "position", newPos.ToVec3());
        return new PlayerPosition
        {
            Id = playerHandle,
            Position = newPos.ToVec3()
        };
    }

    public PlayerLeft RemovePlayer(Player player) => RemovePlayer(player.Handle);

    public PlayerLeft RemovePlayer(int playerHandle)
    {
        if (!_players.TryRemove(playerHandle, out _))
            _logger.LogError("Failed to remove player {PlayerHandle} from ConcurrentDic", playerHandle);

        return new PlayerLeft
        {
            Id = playerHandle
        };
    }
    
    public PlayerUpdate UpdatePlayerMetaData(int playerHandle, Dictionary<string, object> additionalInfo)
    {
        foreach (var kvp in additionalInfo)
        {
            UpdatePlayerDataInternal(playerHandle, kvp.Key, ValueFromObject(kvp.Value));
        }

        return new PlayerUpdate
        {
            Id = playerHandle,
            Data = _players[playerHandle]
        };
    }
    
    public List<int> GetRandomPlayerHandles(int maxPlayers = 10)
    {
        var playerHandles = _players.Keys.ToList();
        var random = new Random();
        var randomPlayerHandles = new List<int>();

        if (playerHandles.Count == 0)
        {
            return randomPlayerHandles;
        }

        var maxPlayersCount = Math.Min(maxPlayers, playerHandles.Count);

        for (var i = 0; i < random.Next(1, maxPlayersCount + 1); i++)
        {
            var randomIndex = random.Next(0, playerHandles.Count);
            randomPlayerHandles.Add(playerHandles[randomIndex]);
        }

        return randomPlayerHandles;
    }

    public List<int> GetAllPlayers() => _players.Keys.ToList();
}