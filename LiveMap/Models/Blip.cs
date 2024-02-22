using System.Text.Json.Serialization;

namespace LiveMap.Models;

public struct Blip
{
    [JsonPropertyName("sprite")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Sprite { get; set; }

    [JsonPropertyName("pos")] public Position Pos { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }
}

public struct Position
{
    [JsonPropertyName("x")] public float X { get; set; }

    [JsonPropertyName("y")] public float Y { get; set; }

    [JsonPropertyName("z")] public float Z { get; set; }

    public double DistanceToSquared(Position otherPosition)
    {
        var newX = Math.Pow(X - otherPosition.X, 2);
        var newY = Math.Pow(Y - otherPosition.Y, 2);
        var newZ = Math.Pow(Z - otherPosition.Z, 2);
        return Math.Sqrt(newX + newY + newZ);
    }

    public bool Equals(Position otherPos)
    {
        var x = Math.Abs(X - otherPos.X) < 0.001;
        var y = Math.Abs(Y - otherPos.Y) < 0.001;
        var z = Math.Abs(Z - otherPos.Z) < 0.001;
        return x && y && z;
    }
}