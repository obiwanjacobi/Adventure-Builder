using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldGrainState
{
    public bool IsLoaded { get; set; }
    public string Name { get; set; } = String.Empty;
    public List<long> PassageIds { get; set; } = [];
}

//[StorageProvider(ProviderName = "GameWorld")]
public sealed class WorldGrain : Grain<WorldGrainState>, IWorldGrain
{
    private readonly IGrainFactory _factory;
    private readonly IAdventureClient _client;

    public WorldGrain(IGrainFactory factory, IAdventureClient client)
    {
        _factory = factory;
        _client = client;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
            var key = WorldKey.Parse(this.GetPrimaryKeyString());
            var world = await _client.GetAdventureWorldSummaryAsync(key.WorldId, cancellationToken);
            State.Name = world.Name;
            State.PassageIds = world.Passages.Select(p => p.Id).ToList();

            // TODO: seed the world
            // - spawn npcs in passage
            var spawnedNpcs = NPC.SpawnNPCs(world);
            await AssignExtaInfoAsync(spawnedNpcs);
            await WriteStateAsync();
        }
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<IPassageGrain> Start(IPlayerGrain player)
    {
        ThrowIfNotLoaded();
        var startPassage = await GetPassage(State.PassageIds[0]);
        await player.EnterPassage(startPassage);
        return startPassage;
    }

    public Task<IPassageGrain> GetPassage(long passageId)
    {
        ThrowIfNotLoaded();
        if (!State.PassageIds.Contains(passageId))
            throw new ArgumentException($"Illegal Passage Id: {passageId} not found in this world.", nameof(passageId));

        var worldKey = WorldKey.Parse(this.GetPrimaryKeyString());
        var key = new PassageKey(worldKey, passageId);
        var passage = _factory.GetGrain<IPassageGrain>(key);
        return Task.FromResult(passage);
    }

    private async Task AssignExtaInfoAsync(IReadOnlyList<AdventureExtraInfo> extraInfos)
    {
        var worldKey = WorldKey.Parse(this.GetPrimaryKeyString());
        foreach (var extraInfo in extraInfos)
        {
            var key = new PassageKey(worldKey, extraInfo.PassageId);
            var passage = _factory.GetGrain<IPassageGrain>(key);
            await passage.AddExtraInfo(extraInfo);
        }
    }

    private void ThrowIfNotLoaded()
    {
        if (!State.IsLoaded)
            throw new InvalidOperationException("This AdventureWorld has not been loaded.");
    }
}
