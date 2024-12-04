using System.Diagnostics;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PassageKey
{
    public PassageKey(WorldKey worldKey, long passageId)
    {
        WorldKey = worldKey;
        PassageId = passageId;
    }
    public WorldKey WorldKey { get; }
    public long PassageId { get; }

    public static PassageKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 3);
        return new PassageKey(
            new WorldKey(parts[0], parts[1]),
            Int64.Parse(parts[2])
        );
    }

    public static implicit operator string(PassageKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, PassageId);
}