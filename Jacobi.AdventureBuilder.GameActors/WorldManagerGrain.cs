using System.Diagnostics;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldManagerState
{
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

    public async Task<IWorldGrain> CreateWorld(string worldId)
    {
        if (State.WorldsById.TryGetValue(worldId, out var world))
        {
            Debug.WriteLine($"Re-created: {worldId}");
            return world;
        }

        // load adventure meta data
        var worldInfo = await this.client.GetAdventureWorldAsync(worldId);
        var fullWorldId = worldInfo.Id + "-" + Guid.NewGuid().ToString();
        world = this.factory.GetGrain<IWorldGrain>(fullWorldId);
        await world.Load(worldInfo);

        State.WorldsById.Add(worldId, world);
        return world;
    }

    public Task<Option<IWorldGrain>> FindWorld(string worldNameOrId)
    {
        if (State.WorldsById.TryGetValue(worldNameOrId, out var world))
            return Task.FromResult(Option<IWorldGrain>.Some(world));

        // find by name
        return Task.FromResult(Option<IWorldGrain>.None);
    }
}
