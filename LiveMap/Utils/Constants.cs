using System.Reflection;

namespace LiveMap.Utils;

public static class Constants
{
    public static readonly string DefaultBlipFile = "blips.json";

    public static readonly string LiveMapUpdateUrl = "https://api.github.com/repos/TGRHavoc/live_map/releases/latest";
    public static readonly Version LiveMapVersion = Assembly.GetExecutingAssembly().GetName().Version;

    public static class Config
    {
        public static readonly string Debug = "livemap_debug";
        public static readonly string BlipFile = "livemap_blipFile";
        public static readonly string AccessControlOrigin = "livemap_accessControl";
    }

    public static class Events
    {
        public static readonly string GeneratedBlips = "livemap:blipsGenerated";
        public static readonly string AddBlip = "livemap:AddBlip";
        public static readonly string UpdateBlip = "livemap:UpdateBlip";
    }
    
    public static class Sse
    {
        public static readonly string RefreshBlips = "refreshBlips";
        public static readonly string AddBlip = "addBlip";
        public static readonly string UpdateBlip = "updateBlip";
        public static readonly string RemoveBlip = "removeBlip";
    }
    
}