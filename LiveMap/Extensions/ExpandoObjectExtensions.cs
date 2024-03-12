using System.Dynamic;
using System.Reflection;
using System.Text.Json.Serialization;
using CitizenFX.Core;

namespace LiveMap.Extensions;

public static class ExpandoObjectExtensions
{
    public static T FromExpando<T>(this ExpandoObject expando) where T : class, new()
    {
        var properties = typeof(T)
            .GetProperties()
            .Where(pi => pi.CanWrite && !pi.GetIndexParameters().Any())
            // Use the json property name if it exists, otherwise use the property name
            .ToDictionary(
                pi => pi.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name?.ToLower() ?? pi.Name.ToLower());

        //Debug.WriteLine("Properties: " + properties.Count);

        var obj = new T();
        foreach (var kvp in expando)
        {
            var name = kvp.Key.ToLower().Replace("_", "");
            var val = kvp.Value;
            //Debug.WriteLine($"Property: {name} Value: {val}");

            if (val != null && properties.TryGetValue(name, out var prop))
                if (prop.PropertyType.IsAssignableFrom(val.GetType()))
                {
                    prop.SetValue(obj, val);
                    //Debug.WriteLine($"Set property: {name} to {val}");
                }
        }

        return obj;
    }
}