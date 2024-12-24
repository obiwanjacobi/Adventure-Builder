using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct AssetKey
{
    public const string Tag = "ast";

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
        Debug.Assert(parts[2] == Tag);
        return new AssetKey(
            new WorldKey(parts[0], parts[1]),
            Int64.Parse(parts[3])
        );
    }

    public static implicit operator string(AssetKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, Tag, AssetId);

    public static bool IsValidKey(string key)
        => key.Contains($"{Key.Separator}{Tag}{Key.Separator}");

    public static Option<AssetKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<AssetKey>.None;

        return Option<AssetKey>.Some(Parse(key));
    }
}
