namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerGrain")]
public interface IPlayerGrain : IPassageOccupantGrain, IGrainWithStringKey
{
    [Alias("Play")]
    Task<GameCommandResult> Play(IWorldGrain world, IPassageGrain passage, GameCommand command);
}
