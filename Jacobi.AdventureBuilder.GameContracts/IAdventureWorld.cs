using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IAdventureWorld")]
public interface IAdventureWorld : IGrainWithStringKey
{
    [Alias("Load")]
    Task Load(AdventureWorldInfo world);
    [Alias("Start")]
    Task<IRoomGrain> Start(IPlayerGrain player);
    [Alias("Stop")]
    Task Stop();
}
