using System.Runtime.InteropServices;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameClient;

public sealed class AdventureGameClient
{
    private readonly IGrainFactory factory;

    public AdventureGameClient(IGrainFactory factory)
        => this.factory = factory;

    public IWorldManager WorldManager
    {
        get
        {
            var guidAttr = typeof(IWorldManager)
                .GetCustomAttributes(false)
                .OfType<GuidAttribute>()
                .Single();

            return this.factory.GetGrain<IWorldManager>(Guid.Parse(guidAttr.Value.AsSpan()));
        }
    }
}
