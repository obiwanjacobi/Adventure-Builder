using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public class AssetGrainState : AmInPassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventureAssetInfo? AssetInfo { get; set; }
}

public sealed class AssetGrain : AmInPassageGrain<AssetGrainState>, IAssetGrain
{
    private readonly IAdventureClient _client;

    public AssetGrain(IAdventureClient client)
        => _client = client;

    protected override string Name
        => State.AssetInfo!.Name;
    protected override string Description
        => State.AssetInfo!.Description;

    public async override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = AssetKey.Parse(this.GetPrimaryKeyString());
            State.AssetInfo = await _client.GetAdventureAsset(key.WorldKey.WorldId, key.AssetId, cancellationToken);
        }

        await base.OnActivateAsync(cancellationToken);
    }
}
