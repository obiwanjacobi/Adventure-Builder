using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class AdventureWorldState
{
    public string WorldId { get; set; } = String.Empty;
}

public sealed class AdventureWorld : Grain<AdventureWorldState>, IAdventureWorld
{
    private readonly IGrainFactory factory;

    public AdventureWorld(IGrainFactory factory)
        => this.factory = factory;

    public string Id
        => this.State.WorldId;

    private AdventureWorldInfo? adventureWorld;
    public Task Load(AdventureWorldInfo world)
    {
        this.adventureWorld = world;
        return Task.CompletedTask;
    }

    public async Task Start(IPlayerGrain player)
    {
        Debug.Assert(this.adventureWorld is not null);
        var startRoom = this.factory.GetGrain<IRoomGrain>(this.adventureWorld.StartRoom.Id);
        await player.EnterRoom(startRoom);
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }
}
