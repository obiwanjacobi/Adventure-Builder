using System.Diagnostics;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PlayerKey
{
    public PlayerKey(Guid accountId, string nickname)
    {
        AccountId = accountId;
        Nickname = nickname;
    }
    public Guid AccountId { get; }
    public string Nickname { get; }

    public static PlayerKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 2);
        return new PlayerKey(Guid.Parse(parts[0]), parts[1]);
    }

    public static implicit operator string(PlayerKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(AccountId, Nickname);
}