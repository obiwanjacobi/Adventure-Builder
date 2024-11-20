using System.Runtime.InteropServices;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameClient;

public sealed class AdventureGameClient
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
    {
        return this.factory.GetGrain<IPlayerGrain>(playerId);
    }
}
