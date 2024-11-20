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

    public async Task<IAdventureWorldGrain> CreateWorld(string adventureId)
    {
        // load adventure meta data
        var worldInfo = await this.client.GetAdventureWorldAsync(adventureId);
        var worldId = worldInfo.Id + "-" + Guid.NewGuid().ToString();
        var world = this.factory.GetGrain<IAdventureWorldGrain>(worldId);
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
