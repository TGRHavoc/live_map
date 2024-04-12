using System.Reflection;

namespace LiveMap.Shared;

public static class Constants
{
    public const string DefaultBlipFile = "blips.json";

    public const string LiveMapUpdateUrl = "https://api.github.com/repos/TGRHavoc/live_map/releases/latest";

    public static class Config
    {
        public const string Debug = "livemap_debug";
        public const string BlipFile = "livemap_blipFile";
        public const string AccessControlOrigin = "livemap_accessControl";
    }

    public static class Events
    {
        public static class Internal
        {
            public const string BlipsRequested = "livemap:__blipsRequested";
        }
        public static class Server
        {
            public const string GeneratedBlips = "livemap:blipsGenerated";
            public const string AddBlip = "livemap:addBlip";
            public const string UpdateBlip = "livemap:updateBlip";
            
            public const string PlayerSpawned = "livemap:playerSpawned";
            public const string RemovePlayer = "livemap:removePlayer";
            
            public const string AddPlayerData = "livemap:addPlayerData";
            public const string UpdatePlayerData = "livemap:updatePlayerData";
            public const string RemovePlayerData = "livemap:removePlayerData";
            
            public const string AddPlayerIdentifier = "livemap:addPlayerIdentifier";
            public const string RemovePlayerIdentifier = "livemap:removePlayerIdentifier";
            public const string UpdatePlayerIdentifier = "livemap:updatePlayerIdentifier";
            
        }

        public static class Client
        {
            public const string GenerateBlips = "livemap:getBlipsFromClient";
        }
        
    }
    
    public static class Sse
    {
        public const string RefreshBlips = "refreshBlips";
        public const string AddBlip = "addBlip";
        public const string UpdateBlip = "updateBlip";
        public const string RemoveBlip = "removeBlip";
        
        public const string PlayerJoin = "playerJoin";
        public const string PlayerLeave = "playerLeave";
        public const string PlayerChange = "playerUpdate";
        public const string PlayerMove = "playerMove";
    }
    
}