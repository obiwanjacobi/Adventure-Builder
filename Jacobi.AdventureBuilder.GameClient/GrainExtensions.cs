using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameClient;

public static class GrainExtensions
{
    public static IWorldGrain GetWorld(this IGrainFactory factory, WorldKey key)
        => factory.GetGrain<IWorldGrain>(key);

    public static IPassageGrain GetPassage(this IGrainFactory factory, PassageKey key)
        => factory.GetGrain<IPassageGrain>(key);

    public static Option<IPassageOccupant> GetPassageOccupant(this IGrainFactory factory, string key, out string? tag)
    {
        if (PlayerKey.IsValidKey(key))
        {
            tag = PlayerKey.Tag;
            return Option<IPassageOccupant>.Some(factory.GetGrain<IPlayerGrain>(key));
        }

        if (NonPlayerCharacterKey.IsValidKey(key))
        {
            tag = NonPlayerCharacterKey.Tag;
            return Option<IPassageOccupant>.Some(factory.GetGrain<INonPlayerCharacterGrain>(key));
        }

        if (AssetKey.IsValidKey(key))
        {
            tag = AssetKey.Tag;
            return Option<IPassageOccupant>.Some(factory.GetGrain<IAssetGrain>(key));
        }

        tag = null;
        return Option<IPassageOccupant>.None;
    }
}
