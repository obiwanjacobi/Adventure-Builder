using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class AdventureWorldState
{
    public string WorldId { get; set; } = String.Empty;
}

public sealed class AdventureWorld : Grain<AdventureWorldState>, IAdventureWorldGrain
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
        return WriteStateAsync();
    }

    public async Task<IRoomGrain> Start(IPlayerGrain player)
    {
        Debug.Assert(this.adventureWorld is not null);
        var startRoom = await GetOrCreateRoom(this.adventureWorld.StartRoom);
        await player.EnterRoom(startRoom);
        return startRoom;
    }

    public Task<IRoomGrain> GetRoom(long roomId)
    {
        ThrowIfNotLoaded();
        var room = this.adventureWorld!.Rooms.Find(r => r.Id == roomId).SingleOrDefault()
            ?? throw new ArgumentOutOfRangeException(nameof(roomId), "Illegal Room Id.");
        return GetOrCreateRoom(room);
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

    private void ThrowIfNotLoaded()
    {
        if (this.adventureWorld is null)
            throw new InvalidOperationException("This AdventureWorld has not been loaded.");
    }
}
