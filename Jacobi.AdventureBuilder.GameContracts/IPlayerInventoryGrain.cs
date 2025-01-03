namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerInventoryGrain")]
public interface IPlayerInventoryGrain : IGrainWithStringKey
{
    Task<IReadOnlyList<IAssetGrain>> Assets();
    Task Add(IAssetGrain asset);
    Task Remove(IAssetGrain asset);
    Task Clear();
}
