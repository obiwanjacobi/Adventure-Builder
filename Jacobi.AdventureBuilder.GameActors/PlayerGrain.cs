using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrain : Grain, IPlayerGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _playerInfo = new(this.GetPrimaryKey(), Guid.Empty, "John Doe");

        return base.OnActivateAsync(cancellationToken);
    }

    private PlayerInfo? _playerInfo;

    public ValueTask<PlayerInfo> GetPlayerInfo()
        => ValueTask.FromResult(_playerInfo!);

    public ValueTask SetPlayerInfo(Guid accountId, string nickname)
    {
        _playerInfo = new PlayerInfo(this.GetPrimaryKey(), accountId, nickname);
        return ValueTask.CompletedTask;
    }

    private IRoomGrain? _room;

    public ValueTask<IRoomGrain?> Room()
        => ValueTask.FromResult(_room);

    public ValueTask EnterRoom(IRoomGrain room)
    {
        _room = room;
        return ValueTask.CompletedTask;
    }

    public Task<GameCommandResult> Play(GameCommand command)
    {
        return Task.FromResult(new GameCommandResult());
    }
}
