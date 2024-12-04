using System.Runtime.InteropServices;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameClient;

public sealed class AdventureGameClient : IGame
{
    private readonly IGrainFactory factory;

    public AdventureGameClient(IClusterClient factory)
        => this.factory = factory;

    public IWorldManagerGrain WorldManager
    {
        get
        {
            var guidAttr = typeof(IWorldManagerGrain)
                .GetCustomAttributes(false)
                .OfType<GuidAttribute>()
                .Single();

            return this.factory.GetGrain<IWorldManagerGrain>(Guid.Parse(guidAttr.Value.AsSpan()));
        }
    }

    public IPlayerGrain GetPlayer(string playerId)
        => this.factory.GetGrain<IPlayerGrain>(playerId);

    public IWorldGrain GetWorld(WorldKey key)
        => this.factory.GetGrain<IWorldGrain>(key);

    public IPassageGrain GetPassage(PassageKey key)
        => this.factory.GetGrain<IPassageGrain>(key);
}
