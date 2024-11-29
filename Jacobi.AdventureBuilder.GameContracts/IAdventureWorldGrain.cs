using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IAdventureWorld")]
public interface IAdventureWorldGrain : IGrainWithStringKey
{
    [Alias("Load")]
    Task Load(AdventureWorldInfo world);
    [Alias("Start")]
    Task<IPassageGrain> Start(IPlayerGrain player);
    [Alias("Stop")]

    Task<IPassageGrain> GetPassage(long passageId);
}
