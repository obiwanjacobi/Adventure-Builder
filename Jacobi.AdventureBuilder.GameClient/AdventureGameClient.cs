using System.Runtime.InteropServices;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameClient;

public sealed class AdventureGameClient : IGame
{
    private readonly IGrainFactory _factory;

    public AdventureGameClient(IClusterClient factory)
        => _factory = factory;

    public IWorldManagerGrain WorldManager
    {
        get
        {
            var guidAttr = typeof(IWorldManagerGrain)
                .GetCustomAttributes(false)
                .OfType<GuidAttribute>()
                .Single();

            return _factory.GetGrain<IWorldManagerGrain>(Guid.Parse(guidAttr.Value.AsSpan()));
        }
    }

    public IPlayerGrain GetPlayer(string playerId)
    {
        // TODO: AccountId
        var key = new PlayerKey(Guid.Empty, playerId);
        return _factory.GetGrain<IPlayerGrain>(key);
    }

    public IWorldGrain GetWorld(WorldKey key)
        => _factory.GetGrain<IWorldGrain>(key);

    public IPassageGrain GetPassage(PassageKey key)
        => _factory.GetGrain<IPassageGrain>(key);

    public IAmInPassage GetAmInPassage(string key)
    {
        if (PlayerKey.IsValidKey(key))
            return _factory.GetGrain<IPlayerGrain>(key);

        if (NonPlayerCharacterKey.IsValidKey(key))
            return _factory.GetGrain<INonPlayerCharacterGrain>(key);

        throw new InvalidOperationException(
            $"The key '{key}' did not resolve to a Player or a Non-Player Character.");
    }
}
