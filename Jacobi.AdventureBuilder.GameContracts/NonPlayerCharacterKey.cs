using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct NonPlayerCharacterKey(WorldKey WorldKey, long NpcId)
{
    public const string Tag = "npc";

    public static NonPlayerCharacterKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 2);
        var worldKey = WorldKey.Construct(keyParts[0]);
        return Construct(worldKey, keyParts[1]);
    }

    public static implicit operator string(NonPlayerCharacterKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join<WorldKey>(WorldKey, Tag, NpcId);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<NonPlayerCharacterKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<NonPlayerCharacterKey>.None;

        return Option<NonPlayerCharacterKey>.Some(Parse(key));
    }

    private static NonPlayerCharacterKey Construct(WorldKey worldKey, string[] parts)
    {
        Debug.Assert(parts.Length == 2);
        Debug.Assert(parts[0] == Tag);
        return new NonPlayerCharacterKey(
            worldKey,
            Int64.Parse(parts[1])
        );
    }
}
