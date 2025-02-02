﻿using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct PlayerKey(Guid AccountId, string Nickname)
{
    public const string Tag = "p";

    //public static PlayerKey Parse(string key)
    //{
    //    var keyParts = Key.Split(key);
    //    Debug.Assert(keyParts.Length == 1);
    //    var parts = keyParts[0];
    //    Debug.Assert(parts.Length == 3);
    //    Debug.Assert(parts[0] == Tag);
    //    return new PlayerKey(Guid.Parse(parts[1]), parts[2]);
    //}

    public static PlayerKey Parse(string key)
    {
        var keyParts = Key.Split(key);
        Debug.Assert(keyParts.Length == 1);
        var parts = keyParts[0];
        Debug.Assert(parts.Length == 2);
        Debug.Assert(parts[0] == Tag);
        return new PlayerKey(Guid.Empty, parts[1]);
    }

    public static implicit operator string(PlayerKey key)
        => key.ToString();
    //public override string ToString()
    //    => Key.Join(Tag, AccountId, Nickname);
    public override string ToString()
        => Key.Join(Tag, Nickname);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<PlayerKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<PlayerKey>.None;

        return Option<PlayerKey>.Some(Parse(key));
    }
}
