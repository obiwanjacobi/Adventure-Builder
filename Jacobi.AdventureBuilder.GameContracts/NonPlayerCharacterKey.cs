using System.Diagnostics;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct NonPlayerCharacterKey
{
    public NonPlayerCharacterKey(WorldKey worldKey, long npcId)
    {
        WorldKey = worldKey;
        NpcId = npcId;
    }
    public WorldKey WorldKey { get; }
    public long NpcId { get; }

    public static NonPlayerCharacterKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 3);
        return new NonPlayerCharacterKey(
            new WorldKey(parts[0], parts[1]),
            Int64.Parse(parts[2])
        );
    }

    public static implicit operator string(NonPlayerCharacterKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, NpcId);
}
