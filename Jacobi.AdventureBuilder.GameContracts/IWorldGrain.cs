namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IWorldGrain")]
public interface IWorldGrain : IGrainWithStringKey
{
    [Alias("Start")]
    Task<IPassageGrain> Start(IPlayerGrain player);

    [Alias("GetPassage")]
    Task<IPassageGrain> GetPassage(long passageId);
}
