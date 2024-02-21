using System;
using System.Reflection;

namespace LiveMap.Utils
{
    public static class Constants
    {
        public static class Config
        {
            public static readonly string Debug = "livemap_debug";
            public static readonly string BlipFile = "livemap_blipFile";
        }
        
        public static readonly string DefaultBlipFile = "blips.json";
        
        public static readonly string LiveMapUpdateUrl = "https://api.github.com/repos/TGRHavoc/live_map/releases/latest";
        public static readonly Version LiveMapVersion = Assembly.GetExecutingAssembly().GetName().Version;
    }
}