namespace Jacobi.AdventureBuilder.GameContracts;

public interface IPlayerGrain : IGrainWithGuidKey
{
    ValueTask<PlayerInfo> GetPlayerInfo();
    ValueTask SetPlayerInfo(Guid accountId, string nickname);

    ValueTask<IRoomGrain?> Room();
    ValueTask EnterRoom(IRoomGrain room);

    Task<GameCommandResult> Play(GameCommand command);
}
