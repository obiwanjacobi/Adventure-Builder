namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IAssetGrain")]
public interface IAssetGrain : IPassageOccupant, IGrainWithStringKey
{
    Task<IReadOnlyList<string>> CommandIds();
}
