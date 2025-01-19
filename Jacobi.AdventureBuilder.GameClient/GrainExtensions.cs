using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameClient;

public static class GrainExtensions
{
    public static T GetSingleton<T>(this IGrainFactory factory) where T : IGrainWithGuidKey
        => factory.GetGrain<T>(Guid.Empty);

    public static IWorldGrain GetWorld(this IGrainFactory factory, string key)
        => factory.GetGrain<IWorldGrain>(key);

    public static IPassageGrain GetPassage(this IGrainFactory factory, string key)
        => factory.GetGrain<IPassageGrain>(key);

    public static IPlayerGrain GetPlayer(this IGrainFactory factory, string key)
        => factory.GetGrain<IPlayerGrain>(key);

    public static IPlayerLogGrain GetPlayerLog(this IGrainFactory factory, IPlayerGrain player)
        => factory.GetGrain<IPlayerLogGrain>(player.GetPrimaryKeyString());
    public static IPlayerLogGrain GetPlayerLog(this IGrainFactory factory, string playerKey)
        => factory.GetGrain<IPlayerLogGrain>(playerKey);

    public static IPlayerInventoryGrain GetPlayerInventory(this IGrainFactory factory, IPlayerGrain player)
        => factory.GetGrain<IPlayerInventoryGrain>(player.GetPrimaryKeyString());
    public static IPlayerInventoryGrain GetPlayerInventory(this IGrainFactory factory, string playerKey)
        => factory.GetGrain<IPlayerInventoryGrain>(playerKey);

    public static Option<IPassageOccupantGrain> GetPassageOccupant(this IGrainFactory factory, string key, out string? tag)
    {
        if (PlayerKey.IsValidKey(key))
        {
            tag = PlayerKey.Tag;
            return Option<IPassageOccupantGrain>.Some(factory.GetGrain<IPlayerGrain>(key));
        }

        if (NonPlayerCharacterKey.IsValidKey(key))
        {
            tag = NonPlayerCharacterKey.Tag;
            return Option<IPassageOccupantGrain>.Some(factory.GetGrain<INonPlayerCharacterGrain>(key));
        }

        if (AssetKey.IsValidKey(key))
        {
            tag = AssetKey.Tag;
            return Option<IPassageOccupantGrain>.Some(factory.GetGrain<IAssetGrain>(key));
        }

        tag = null;
        return Option<IPassageOccupantGrain>.None;
    }
}
