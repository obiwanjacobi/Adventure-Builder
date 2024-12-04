using System.Diagnostics;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldManagerState
{
    // this should probably be persisted into a database of running games
    public Dictionary<string, IWorldGrain> WorldsById { get; } = [];
}

public sealed class WorldManagerGrain : Grain<WorldManagerState>, IWorldManagerGrain
{
    private readonly IGrainFactory factory;
    private readonly IAdventureClient client;

    public WorldManagerGrain(IGrainFactory factory, IAdventureClient client)
    {
        this.factory = factory;
        this.client = client;
    }

    public Task<IWorldGrain> CreateNewWorld(string worldId)
    {
        // for now
        if (State.WorldsById.TryGetValue(worldId, out var world))
        {
            Debug.WriteLine($"Re-created: {worldId}");
            return Task.FromResult(world);
        }

        // load adventure meta data
        //var worldInfo = await this.client.GetAdventureWorldAsync(worldId);
        var key = new WorldKey(worldId, Guid.NewGuid().ToString());

        world = this.factory.GetGrain<IWorldGrain>(key);
        //await world.Load(worldInfo);

        State.WorldsById.Add(worldId, world);
        return Task.FromResult(world);
    }
}
