using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PlayerInventoryKey(Guid AccountId, string Nickname)
{
    public const string Tag = "pi";

    public static PlayerInventoryKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 1);
        var parts = keyParts[0];
        Debug.Assert(parts.Length == 3);
        Debug.Assert(parts[0] == Tag);
        return new PlayerInventoryKey(Guid.Parse(parts[1]), parts[2]);
    }

    public static implicit operator string(PlayerInventoryKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(Tag, AccountId, Nickname);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<PlayerInventoryKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<PlayerInventoryKey>.None;

        return Option<PlayerInventoryKey>.Some(Parse(key));
    }
}
