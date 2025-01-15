namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IAssetGrain")]
public interface IAssetGrain : IPassageOccupantGrain, IGrainWithStringKey
{
    Task<IReadOnlyList<string>> CommandKinds();
}
