using System.ComponentModel;
using CitizenFX.Server.Native;
using LiveMap.Shared;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Utils;

public static class Config
{
    
    private static LogLevel? _loglevelEnum;
    public static LogLevel LiveMapLogLevel
    {
        get
        {
            // If LogLevel is already set, return it
            if (_loglevelEnum != null) return _loglevelEnum.Value;
            var logLevel = GetConvarValue(Constants.Config.Debug, "off");

            switch (logLevel)
            {
                case "[all]":
                case "trace":
                    _loglevelEnum = LogLevel.Trace;
                    break;
                case "debug":
                    _loglevelEnum = LogLevel.Debug;
                    break;
                case "info":
                    _loglevelEnum = LogLevel.Information;
                    break;
                case "warn":
                    _loglevelEnum = LogLevel.Warning;
                    break;
                case "error":
                    _loglevelEnum = LogLevel.Error;
                    break;
                case "critical":
                    _loglevelEnum = LogLevel.Critical;
                    break;
                default:
                    _loglevelEnum = LogLevel.None;
                    break;
            }

            return _loglevelEnum.Value;
        }
    }
    
    public static T GetConfigKeyValue<T>(string metadataKey, int index, T defaultValue, ILogger? logger = null)
    {
        var result = defaultValue;

        try
        {
            var input = Natives.GetResourceMetadata(LiveMap.ResourceName, metadataKey, index);
            result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
        }
        catch (Exception)
        {
            logger?.LogError("Failed to parse {MetadataKey}. Using default value {DefaultValue}", metadataKey,
                defaultValue);
        }

        return result;
    }

    public static string GetConvarValue(string convar, string defaultValue)
        => Natives.GetConvar(convar, defaultValue);
}