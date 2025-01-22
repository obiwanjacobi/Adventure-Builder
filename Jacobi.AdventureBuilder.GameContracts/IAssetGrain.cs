using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IAssetGrain")]
public interface IAssetGrain : IPassageOccupantGrain, IGrainWithStringKey
{
    [ReadOnly]
    Task<IReadOnlyList<string>> CommandKinds();
}
