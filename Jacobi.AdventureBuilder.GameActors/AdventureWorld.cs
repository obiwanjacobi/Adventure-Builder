using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class AdventureWorldState
{
    public AdventureWorldInfo? AdventureWorld { get; set; }
}

public sealed class AdventureWorld : Grain<AdventureWorldState>, IAdventureWorldGrain
{
    private readonly IGrainFactory factory;

    public AdventureWorld(IGrainFactory factory)
        => this.factory = factory;

    public string Id
        => this.State.AdventureWorld?.Id
            ?? throw new InvalidOperationException("The World has not been Loaded.");

    public Task Load(AdventureWorldInfo world)
    {
        this.State.AdventureWorld = world;
        return WriteStateAsync();
    }

    public async Task<IPassageGrain> Start(IPlayerGrain player)
    {
        ThrowIfNotLoaded();
        // TODO: seed the world
        // - spawn npcs in passage
        var startPassage = await GetOrCreatePassage(this.State.AdventureWorld!.StartPassage);
        await player.EnterPassage(startPassage);
        return startPassage;
    }

    public Task<IPassageGrain> GetPassage(long passageId)
    {
        ThrowIfNotLoaded();
        var passage = this.State.AdventureWorld!.Passages.Find(r => r.Id == passageId).SingleOrDefault()
            ?? throw new ArgumentOutOfRangeException(nameof(passageId), "Illegal Passage Id.");
        return GetOrCreatePassage(passage);
    }

    private async Task<IPassageGrain> GetOrCreatePassage(AdventurePassageInfo passageInfo)
    {
        var passage = this.factory.GetGrain<IPassageGrain>(passageInfo.Id);
        if (await passage.Load(passageInfo))
        {
            // initial load
        }
        return passage;
    }

    private void ThrowIfNotLoaded()
    {
        if (this.State.AdventureWorld is null)
            throw new InvalidOperationException("This AdventureWorld has not been loaded.");
    }
}
