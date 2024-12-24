using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct NonPlayerCharacterKey
{
    public const string Tag = "npc";

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
        Debug.Assert(parts.Length == 4);
        Debug.Assert(parts[2] == Tag);
        return new NonPlayerCharacterKey(
            new WorldKey(parts[0], parts[1]),
            Int64.Parse(parts[3])
        );
    }

    public static implicit operator string(NonPlayerCharacterKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, Tag, NpcId);

    public static bool IsValidKey(string key)
        => key.Contains($"{Key.Separator}{Tag}{Key.Separator}");

    public static Option<NonPlayerCharacterKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<NonPlayerCharacterKey>.None;

        return Option<NonPlayerCharacterKey>.Some(Parse(key));
    }
}
