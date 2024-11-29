namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerGrain")]
public interface IPlayerGrain : IGrainWithStringKey
{
    [Alias("PlayerInfo")]
    Task<PlayerInfo> PlayerInfo();
    [Alias("SetPlayerInfo")]
    Task SetPlayerInfo(Guid accountId, string nickname);

    [Alias("Passage")]
    Task<IPassageGrain?> Passage();
    [Alias("EnterPassage")]
    Task EnterPassage(IPassageGrain room);

    [Alias("Play")]
    Task<GameCommandResult> Play(IAdventureWorldGrain world, GameCommand command);
}
