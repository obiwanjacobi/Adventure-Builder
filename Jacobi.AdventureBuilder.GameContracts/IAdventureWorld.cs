using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameContracts;

public interface IAdventureWorld : IGrainWithStringKey
{
    Task Load(AdventureWorldInfo world);
    Task Start(IPlayerGrain player);
    Task Stop();
}
