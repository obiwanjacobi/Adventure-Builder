using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PassageKey
{
    public const string Tag = "psg";

    public PassageKey(WorldKey worldKey, long passageId)
    {
        WorldKey = worldKey;
        PassageId = passageId;
    }
    public WorldKey WorldKey { get; }
    public long PassageId { get; }

    public static PassageKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 2);
        var worldKey = WorldKey.Construct(keyParts[0]);
        return Construct(worldKey, keyParts[1]);
    }

    public static implicit operator string(PassageKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join<WorldKey>(WorldKey, Tag, PassageId);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<PassageKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<PassageKey>.None;

        return Option<PassageKey>.Some(Parse(key));
    }

    private static PassageKey Construct(WorldKey worldKey, string[] parts)
    {
        Debug.Assert(parts.Length == 2);
        Debug.Assert(parts[0] == Tag);
        return new PassageKey(
            worldKey,
            Int64.Parse(parts[1])
        );
    }
}
