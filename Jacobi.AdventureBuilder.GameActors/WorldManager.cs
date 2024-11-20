using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldManagerState
{
    public Dictionary<string, IAdventureWorld> WorldsById { get; } = [];
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

    public async Task<IAdventureWorld> CreateWorld(string adventureId)
    {
        // load adventure meta data
        var worldInfo = await this.client.GetAdventureWorldAsync(adventureId);
        var worldId = worldInfo.Id + "-" + Guid.NewGuid().ToString();
        var world = this.factory.GetGrain<IAdventureWorld>(worldId);
        await world.Load(worldInfo);

        State.WorldsById.Add(worldId, world);
        return world;
    }

    public Task<Option<IAdventureWorld>> FindWorld(string worldNameOrId)
    {
        if (State.WorldsById.TryGetValue(worldNameOrId, out var world))
            return Task.FromResult(Option<IAdventureWorld>.Some(world));

        // find by name
        return Task.FromResult(Option<IAdventureWorld>.None);
    }
}
