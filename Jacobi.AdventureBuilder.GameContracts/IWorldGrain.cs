namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IWorldGrain")]
public interface IWorldGrain : IGrainWithStringKey
{
    //[Alias("Load")]
    //Task Load(AdventureWorldInfo world);

    [Alias("Start")]
    Task<IPassageGrain> Start(IPlayerGrain player);

    [Alias("GetPassage")]
    Task<IPassageGrain> GetPassage(long passageId);
}
