using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerInventoryGrainState
{
    public bool IsLoaded { get; set; }
    public List<IAssetGrain> Assets { get; set; } = [];
}

internal sealed class PlayerInventoryGrain : Grain<PlayerInventoryGrainState>, IPlayerInventoryGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
        }

        return base.OnActivateAsync(cancellationToken);
    }

    public Task<IReadOnlyList<IAssetGrain>> Assets()
    {
        return Task.FromResult((IReadOnlyList<IAssetGrain>)State.Assets);
    }

    public Task Add(IAssetGrain asset)
    {
        State.Assets.Add(asset);
        return WriteStateAsync();
    }

    public Task Remove(IAssetGrain asset)
    {
        State.Assets.Remove(asset);
        return WriteStateAsync();
    }

    public Task Clear()
    {
        State.Assets.Clear();
        return Task.CompletedTask;
    }
}
