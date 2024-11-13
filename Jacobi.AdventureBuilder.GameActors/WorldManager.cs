using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldManagerState
{
    public Dictionary<string, IAdventureWorld> WorldsById { get; } = [];
}

public sealed class WorldManager : Grain<WorldManagerState>, IWorldManager
{
    private readonly IGrainFactory factory;
    private readonly IAdventureClient client;

    public WorldManager(IGrainFactory factory, IAdventureClient client)
    {
        this.factory = factory;
        this.client = client;
    }

    public Task<IAdventureWorld> CreateWorld(string adventureId)
    {
        // load adventure meta data
        var worldMap = this.client.GetAdventure(adventureId);
        var worldId = worldMap.Name + "-" + Guid.NewGuid().ToString();
        var world = this.factory.GetGrain<IAdventureWorld>(worldId);
        //world.Load(adventureMap);
        State.WorldsById.Add(worldId, world);
        return Task.FromResult(world);
    }

    public ValueTask<Option<IAdventureWorld>> FindWorld(string worldNameOrId)
    {
        if (State.WorldsById.TryGetValue(worldNameOrId, out var world))
            return ValueTask.FromResult(Option<IAdventureWorld>.Some(world));

        // find by name
        return ValueTask.FromResult(Option<IAdventureWorld>.None);
    }
}
