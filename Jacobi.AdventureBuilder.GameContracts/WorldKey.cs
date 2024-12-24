using System.Diagnostics;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

public readonly record struct WorldKey
{
    public const string Tag = "w";

    public WorldKey(string worldId, string instanceId)
    {
        WorldId = worldId;
        InstanceId = instanceId;
    }

    public string WorldId { get; }
    public string InstanceId { get; }

    public static WorldKey Parse(string key)
    {
        var parts = Key.Split(key);
        Debug.Assert(parts.Length == 3);
        Debug.Assert(parts[1] == Tag);
        return new WorldKey(parts[0], parts[2]);
    }

    public static implicit operator string(WorldKey key)
        => key.ToString();
    public override string ToString()
        => Key.Join(WorldId, Tag, InstanceId);

    public static bool IsValidKey(string key)
        => key.Contains($"{Key.Separator}{Tag}{Key.Separator}");

    public static Option<WorldKey> TryParse(string key)
    {
        if (!IsValidKey(key))
            return Option<WorldKey>.None;

        return Option<WorldKey>.Some(Parse(key));
    }
}
