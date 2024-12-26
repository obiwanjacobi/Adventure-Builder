using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PlayerKey
{
    public const string Tag = "ply";

    public PlayerKey(Guid accountId, string nickname)
    {
        AccountId = accountId;
        Nickname = nickname;
    }
    public Guid AccountId { get; }
    public string Nickname { get; }

    public static PlayerKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 1);
        var parts = keyParts[0];
        Debug.Assert(parts.Length == 3);
        Debug.Assert(parts[0] == Tag);
        return new PlayerKey(Guid.Parse(parts[1]), parts[2]);
    }

    public static implicit operator string(PlayerKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(Tag, AccountId, Nickname);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<PlayerKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<PlayerKey>.None;

        return Option<PlayerKey>.Some(Parse(key));
    }
}
