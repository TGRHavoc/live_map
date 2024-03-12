using CitizenFX.Core;

namespace LiveMap.Models;

public class MapVec3
{
    public static MapVec3 Random(int min = -5000, int max = 5000)
    {
        var random = new Random();
        return new MapVec3(random.Next(min, max), random.Next(min, max), random.Next(min, max));
    }
    
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public MapVec3()
    {
        X = 0f;
        Y = 0f;
        Z = 0f;
    }

    public MapVec3(Vec3 vector3)
    {
        X = (float)Math.Round(vector3.X, 2);
        Y = (float)Math.Round(vector3.Y, 2);
        Z = (float)Math.Round(vector3.Z, 2);
    }

    public MapVec3(Vector3 vector3)
    {
        X = (float)Math.Round(vector3.X, 2);
        Y = (float)Math.Round(vector3.Y, 2);
        Z = (float)Math.Round(vector3.Z, 2);
    }

    public MapVec3(float value)
    {
        X = (float)Math.Round(value, 2);
        Y = (float)Math.Round(value, 2);
        Z = (float)Math.Round(value, 2);
    }

    public MapVec3(float x, float y, float z)
    {
        X = (float)Math.Round(x, 2);
        Y = (float)Math.Round(y, 2);
        Z = (float)Math.Round(z, 2);
    }

    public double DistanceToSquared(MapVec3 otherPosition)
    {
        var newX = Math.Pow(X - otherPosition.X, 2);
        var newY = Math.Pow(Y - otherPosition.Y, 2);
        var newZ = Math.Pow(Z - otherPosition.Z, 2);
        return Math.Sqrt(newX + newY + newZ);
    }

    public bool Equals(MapVec3? otherPos)
    {
        if (otherPos == null) return false;

        var x = Math.Abs(X - otherPos.X) < 0.001;
        var y = Math.Abs(Y - otherPos.Y) < 0.001;
        var z = Math.Abs(Z - otherPos.Z) < 0.001;
        return x && y && z;
    }

    public Vec3 ToVec3()
    {
        return new Vec3
        {
            X = X,
            Y = Y,
            Z = Z
        };
    }
}