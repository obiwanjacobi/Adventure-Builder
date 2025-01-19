using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class WorldGrainState
{
    public bool IsLoaded { get; set; }
    public string Name { get; set; } = String.Empty;
    public List<long> PassageIds { get; set; } = [];
    public List<long> NonPlayerCharacterIds { get; set; } = [];
}

//[StorageProvider(ProviderName = "GameWorld")]
public sealed class WorldGrain : Grain<WorldGrainState>, IWorldGrain
{
    private readonly IAdventureClient _client;

    public WorldGrain(IAdventureClient client)
    {
        _client = client;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
            var worldKey = WorldKey.Parse(this.GetPrimaryKeyString());
            var world = await _client.GetAdventureWorldSummaryAsync(worldKey.WorldId, cancellationToken);
            State.Name = world.Name;
            State.PassageIds = world.Passages.Select(p => p.Id).ToList();
            State.NonPlayerCharacterIds = world.NonPlayerCharacters.Select(npc => npc.Id).ToList();

            var context = new GameContext(this);

            // - place npcs and assets in passages
            foreach (var npc in world.NonPlayerCharacters)
            {
                var passage = Placement.PlaceInPassage(world, npc.LinkedPassageIds);
                var passageKey = new PassageKey(worldKey, passage.Id);
                var passageGrain = GrainFactory.GetGrain<IPassageGrain>(passageKey);

                var npcKey = new NonPlayerCharacterKey(worldKey, npc.Id);
                var npcGrain = GrainFactory.GetGrain<INonPlayerCharacterGrain>(npcKey);

                await npcGrain.GotoPassage(context, passageGrain);
            }
            foreach (var asset in world.Assets)
            {
                var passage = Placement.PlaceInPassage(world, asset.LinkedPassageIds);
                var passageKey = new PassageKey(worldKey, passage.Id);
                var passageGrain = GrainFactory.GetGrain<IPassageGrain>(passageKey);

                var assetKey = new AssetKey(worldKey, asset.Id);
                var assetGrain = GrainFactory.GetGrain<IAssetGrain>(assetKey);

                await assetGrain.GotoPassage(context, passageGrain);
            }

            await WriteStateAsync();
        }
        await base.OnActivateAsync(cancellationToken);
    }

    public Task<string> Name()
        => Task.FromResult(State.Name);

    public async Task<IPassageGrain> Start(IPlayerGrain player)
    {
        var log = GrainFactory.GetPlayerLog(player);
        await log.Clear();
        var inventory = GrainFactory.GetPlayerInventory(player);
        await inventory.Clear();

        var startPassage = await GetPassage(State.PassageIds[0]);
        var context = new GameContext(this, player);
        await player.GotoPassage(context, startPassage);
        return startPassage;
    }

    public Task<IPassageGrain> GetPassage(long passageId)
    {
        if (!State.PassageIds.Contains(passageId))
            throw new ArgumentException($"Illegal Passage Id: {passageId} not found in this world.", nameof(passageId));

        var worldKey = WorldKey.Parse(this.GetPrimaryKeyString());
        var key = new PassageKey(worldKey, passageId);
        var passage = GrainFactory.GetGrain<IPassageGrain>(key);
        return Task.FromResult(passage);
    }
}
