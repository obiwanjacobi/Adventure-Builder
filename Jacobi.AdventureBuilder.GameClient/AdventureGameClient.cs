using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameClient;

public sealed class AdventureGameClient //: IGame
{
    private readonly IGrainFactory _factory;

    public AdventureGameClient(IClusterClient factory)
        => _factory = factory;

    public IWorldManagerGrain WorldManager
    {
        get { return _factory.GetSingleton<IWorldManagerGrain>(); }
    }

    public IPlayerGrain GetPlayer(string playerId)
    {
        // TODO: AccountId
        var key = new PlayerKey(Guid.Empty, playerId);
        return _factory.GetGrain<IPlayerGrain>(key);
    }

    public T GetGrain<T>(string key) where T : IGrainWithStringKey
        => _factory.GetGrain<T>(key);
}
