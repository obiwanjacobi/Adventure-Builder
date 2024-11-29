namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerGrain")]
public interface IPlayerGrain : IGrainWithStringKey
{
    [Alias("PlayerInfo")]
    Task<PlayerInfo> PlayerInfo();
    [Alias("SetPlayerInfo")]
    Task SetPlayerInfo(Guid accountId, string nickname);

    [Alias("Room")]
    Task<IPassageGrain?> Room();
    [Alias("EnterRoom")]
    Task EnterRoom(IPassageGrain room);

    [Alias("Play")]
    Task<GameCommandResult> Play(IAdventureWorldGrain world, GameCommand command);
}
