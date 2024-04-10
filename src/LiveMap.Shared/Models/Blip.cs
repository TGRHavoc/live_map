using System.Text.Json.Serialization;

namespace LiveMap.Shared.Models;

public struct Blip
{
    [JsonPropertyName("sprite")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Sprite { get; set; }

    [JsonPropertyName("pos")]
    public MapVec3 Pos { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }
}