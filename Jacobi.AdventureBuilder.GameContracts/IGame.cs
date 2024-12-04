namespace Jacobi.AdventureBuilder.GameContracts;

public interface IGame
{
    IWorldGrain GetWorld(WorldKey key);
    IPassageGrain GetPassage(PassageKey key);
}
