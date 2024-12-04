using System.Diagnostics;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldManagerState
{
    // this should probably be persisted into a database of running games
    public Dictionary<string, IWorldGrain> WorldsById { get; } = [];
}

public sealed class WorldManagerGrain : Grain<WorldManagerState>, IWorldManagerGrain
{
    private readonly IGrainFactory _factory;

    public WorldManagerGrain(IGrainFactory factory)
    {
        _factory = factory;
    }

    public Task<IWorldGrain> CreateNewWorld(string worldId, string name)
    {
        // for now
        if (State.WorldsById.TryGetValue(worldId, out var world))
        {
            Debug.WriteLine($"Re-created: {worldId}");
            return Task.FromResult(world);
        }

        var key = new WorldKey(worldId, name);
        world = _factory.GetGrain<IWorldGrain>(key);

        State.WorldsById.Add(worldId, world);
        return Task.FromResult(world);
    }
}
