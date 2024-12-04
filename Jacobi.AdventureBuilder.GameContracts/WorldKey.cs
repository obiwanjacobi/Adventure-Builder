using System.Diagnostics;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct WorldKey
{
    public WorldKey(string worldId, string instanceId)
    {
        WorldId = worldId;
        InstanceId = instanceId;
    }

    public string WorldId { get; }
    public string InstanceId { get; }

    public static WorldKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 2);
        return new WorldKey(parts[0], parts[1]);
    }

    public static implicit operator string(WorldKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldId, InstanceId);
}
