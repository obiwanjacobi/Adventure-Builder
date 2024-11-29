using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class AdventureWorldState
{
    public string WorldId { get; set; } = String.Empty;
}

public sealed class AdventureWorld : Grain<AdventureWorldState>, IAdventureWorldGrain
{
    private readonly IGrainFactory factory;

    public AdventureWorld(IGrainFactory factory)
        => this.factory = factory;

    public string Id
        => this.State.WorldId;

    private AdventureWorldInfo? adventureWorld;
    public Task Load(AdventureWorldInfo world)
    {
        this.adventureWorld = world;
        return WriteStateAsync();
    }

    public async Task<IPassageGrain> Start(IPlayerGrain player)
    {
        Debug.Assert(this.adventureWorld is not null);
        var startPassage = await GetOrCreatePassage(this.adventureWorld.StartPassage);
        await player.EnterRoom(startPassage);
        return startPassage;
    }

    public Task<IPassageGrain> GetPassage(long passageId)
    {
        ThrowIfNotLoaded();
        var passage = this.adventureWorld!.Passages.Find(r => r.Id == passageId).SingleOrDefault()
            ?? throw new ArgumentOutOfRangeException(nameof(passageId), "Illegal Passage Id.");
        return GetOrCreatePassage(passage);
    }

    private async Task<IPassageGrain> GetOrCreatePassage(AdventurePassageInfo passageInfo)
    {
        var room = this.factory.GetGrain<IPassageGrain>(passageInfo.Id);
        if (await room.Load(passageInfo))
        {
            // initial load
        }
        return room;
    }

    private void ThrowIfNotLoaded()
    {
        if (this.adventureWorld is null)
            throw new InvalidOperationException("This AdventureWorld has not been loaded.");
    }
}
