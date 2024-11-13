namespace Jacobi.AdventureBuilder.GameContracts;

public interface IAdventureWorld : IGrainWithStringKey
{
    Task Start();
    Task Stop();
}
