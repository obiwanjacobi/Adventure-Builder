namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerGrain")]
public interface IPlayerGrain : IAmInPassage, IGrainWithStringKey
{
    [Alias("Play")]
    Task<GameCommandResult> Play(IWorldGrain world, GameCommand command);
}
