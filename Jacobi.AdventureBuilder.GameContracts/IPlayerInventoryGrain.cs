namespace Jacobi.AdventureBuilder.GameContracts;

public interface IPlayerInventoryGrain : IGrainWithStringKey
{
    Task<IReadOnlyList<IAssetGrain>> Assets();
    Task Add(IAssetGrain asset);
}
