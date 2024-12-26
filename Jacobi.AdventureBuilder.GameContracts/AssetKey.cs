﻿using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct AssetKey
{
    public const string Tag = "ast";

    public AssetKey(WorldKey worldKey, long assetId)
    {
        WorldKey = worldKey;
        AssetId = assetId;
    }
    public WorldKey WorldKey { get; }
    public long AssetId { get; }

    public static AssetKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 5);
        Debug.Assert(parts[1] == WorldKey.Tag);
        Debug.Assert(parts[3] == Tag);
        return new AssetKey(
            new WorldKey(parts[0], parts[2]),
            Int64.Parse(parts[4])
        );
    }

    public static implicit operator string(AssetKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldKey, Tag, AssetId);

    public static bool IsValidKey(string key)
        => Key.HasTag(key, Tag);

    public static Option<AssetKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<AssetKey>.None;

        return Option<AssetKey>.Some(Parse(key));
    }
}
