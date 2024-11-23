using System.Diagnostics;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldManagerState
{
    public Dictionary<string, IAdventureWorldGrain> WorldsById { get; } = [];
}

public sealed class WorldManager : Grain<WorldManagerState>, IWorldManagerGrain
{
    private readonly IGrainFactory factory;
    private readonly IAdventureClient client;

    public WorldManager(IGrainFactory factory, IAdventureClient client)
    {
        this.factory = factory;
        this.client = client;
    }

    public async Task<IAdventureWorldGrain> CreateWorld(string worldId)
    {
        if (State.WorldsById.TryGetValue(worldId, out var world))
        {
            Debug.WriteLine($"Re-created: {worldId}");
            return world;
        }

        var ct = new CancellationToken();
        // load adventure meta data
        var worldInfo = await this.client.GetAdventureWorldAsync(worldId, ct);
        var fullWorldId = worldInfo.Id + "-" + Guid.NewGuid().ToString();
        world = this.factory.GetGrain<IAdventureWorldGrain>(fullWorldId);
        await world.Load(worldInfo);

        State.WorldsById.Add(worldId, world);
        return world;
    }

    public Task<Option<IAdventureWorldGrain>> FindWorld(string worldNameOrId)
    {
        if (State.WorldsById.TryGetValue(worldNameOrId, out var world))
            return Task.FromResult(Option<IAdventureWorldGrain>.Some(world));

        // find by name
        return Task.FromResult(Option<IAdventureWorldGrain>.None);
    }
}
