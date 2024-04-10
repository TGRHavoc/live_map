using System.Reflection;

namespace LiveMap.Utils;

public static class Constants
{
    public const string DefaultBlipFile = "blips.json";

    public const string LiveMapUpdateUrl = "https://api.github.com/repos/TGRHavoc/live_map/releases/latest";
    public static readonly Version LiveMapVersion = Assembly.GetExecutingAssembly().GetName().Version;

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
            public const string AddBlip = "livemap:AddBlip";
            public const string UpdateBlip = "livemap:UpdateBlip";
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