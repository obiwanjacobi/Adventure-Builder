using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PlayerLogKey(Guid AccountId, string Nickname)
{
    public const string Tag = "pl";

    public static PlayerLogKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 1);
        var parts = keyParts[0];
        Debug.Assert(parts.Length == 3);
        Debug.Assert(parts[0] == Tag);
        return new PlayerLogKey(Guid.Parse(parts[1]), parts[2]);
    }

    public static implicit operator string(PlayerLogKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(Tag, AccountId, Nickname);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<PlayerLogKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<PlayerLogKey>.None;

        return Option<PlayerLogKey>.Some(Parse(key));
    }
}
