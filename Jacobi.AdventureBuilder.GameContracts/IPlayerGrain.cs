namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerGrain")]
public interface IPlayerGrain : IAmInPassage, IGrainWithStringKey
{
    [Alias("Play")]
    Task<GameCommandResult> Play(IWorldGrain world, IPassageGrain passage, GameCommand command);

    [Alias("Log")]
    Task<IPlayerLogGrain> Log();

    [Alias("Inventory")]
    Task<IPlayerInventoryGrain> Inventory();
}
