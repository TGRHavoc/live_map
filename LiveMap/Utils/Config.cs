using System.ComponentModel;
using CitizenFX.Server.Native;
using Microsoft.Extensions.Logging;

namespace LiveMap.Utils
{
    public static class Config
    {
        public static T GetConfigKeyValue<T>(string metadataKey, int index, T defaultValue, ILogger logger = null)
        {
            var result = defaultValue;
            
            try
            {
                var input = Natives.GetResourceMetadata(LiveMap.ResourceName, metadataKey, index);
                result = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
            }
            catch (Exception)
            {
                logger?.LogError("Failed to parse {MetadataKey}. Using default value {DefaultValue}", metadataKey, defaultValue);
            }

            return result;
        }
    }
}