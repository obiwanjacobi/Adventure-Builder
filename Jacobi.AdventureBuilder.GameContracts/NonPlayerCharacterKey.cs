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
        Debug.Assert(parts.Length == 5);
        Debug.Assert(parts[1] == WorldKey.Tag);
        Debug.Assert(parts[3] == Tag);
        return new NonPlayerCharacterKey(
            new WorldKey(parts[0], parts[2]),
            Int64.Parse(parts[4])
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
