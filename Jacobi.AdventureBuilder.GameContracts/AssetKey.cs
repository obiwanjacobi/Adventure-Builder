using System.Diagnostics;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct AssetKey
{
    public AssetKey(WorldKey worldKey, long assetId)
    {
        WorldKey = worldKey;
        AssetId = assetId;
    }
    public WorldKey WorldKey { get; }
    public long AssetId { get; }

    public static AssetKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 3);
        return new AssetKey(
            new WorldKey(parts[0], parts[1]),
            Int64.Parse(parts[2])
        );
    }

    public static implicit operator string(AssetKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, AssetId);
}
