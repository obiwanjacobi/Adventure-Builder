using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct AssetKey(WorldKey WorldKey, long AssetId)
{
    public const string Tag = "a";

    public static AssetKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 2);
        var worldKey = WorldKey.Construct(keyParts[0]);
        return Construct(worldKey, keyParts[1]);
    }

    public static implicit operator string(AssetKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join<WorldKey>(WorldKey, Tag, AssetId);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<AssetKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<AssetKey>.None;

        return Option<AssetKey>.Some(Parse(key));
    }

    private static AssetKey Construct(WorldKey worldKey, string[] parts)
    {
        Debug.Assert(parts.Length == 2);
        Debug.Assert(parts[0] == Tag);
        return new AssetKey(
            worldKey,
            Int64.Parse(parts[1])
        );
    }
}
