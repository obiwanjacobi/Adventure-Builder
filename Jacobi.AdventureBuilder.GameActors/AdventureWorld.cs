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

    public async Task<IRoomGrain> Start(IPlayerGrain player)
    {
        Debug.Assert(this.adventureWorld is not null);
        var startRoom = await GetOrCreateRoom(this.adventureWorld.StartRoom);
        await player.EnterRoom(startRoom);
        return startRoom;
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }

    private async Task<IRoomGrain> GetOrCreateRoom(AdventureRoomInfo roomInfo)
    {
        var room = this.factory.GetGrain<IRoomGrain>(roomInfo.Id);
        if (await room.Load(roomInfo))
        {
            // initial load
        }
        return room;
    }
}
