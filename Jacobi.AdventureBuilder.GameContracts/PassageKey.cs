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
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 5);
        Debug.Assert(parts[1] == WorldKey.Tag);
        Debug.Assert(parts[3] == Tag);
        return new PassageKey(
            new WorldKey(parts[0], parts[2]),
            Int64.Parse(parts[4])
        );
    }

    public static implicit operator string(PassageKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, Tag, PassageId);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<PassageKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<PassageKey>.None;

        return Option<PassageKey>.Some(Parse(key));
    }
}
