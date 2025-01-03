using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public class AssetGrainState : PassageOccupantGrainState
{
    public bool IsLoaded { get; set; }
    public AdventureAssetInfo? AssetInfo { get; set; }
}

public sealed class AssetGrain : PassageOccupantGrain<AssetGrainState>, IAssetGrain
{
    private readonly IAdventureClient _client;

    public AssetGrain(IAdventureClient client)
    {
        _client = client;
    }

    public override Task<string> Name()
        => Task.FromResult(State.AssetInfo!.Name);
    public override Task<string> Description()
        => Task.FromResult(State.AssetInfo!.Description);

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

    public Task<IReadOnlyList<string>> CommandIds()
    {
        return Task.FromResult((IReadOnlyList<string>)State.AssetInfo!.Properties
            .Where(prop => prop.Name == "cmd")
            .Select(prop => prop.Value)
            .ToList()
        );
    }
}
