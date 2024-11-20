using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrain : Grain, IPlayerGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _playerInfo = new(this.GetPrimaryKeyString(), Guid.Empty, "John Doe");

        return base.OnActivateAsync(cancellationToken);
    }

    private PlayerInfo? _playerInfo;

    public Task<PlayerInfo> PlayerInfo()
        => Task.FromResult(_playerInfo!);

    public Task SetPlayerInfo(Guid accountId, string nickname)
    {
        _playerInfo = new PlayerInfo(this.GetPrimaryKeyString(), accountId, nickname);
        return Task.CompletedTask;
    }

    private IRoomGrain? _room;

    public Task<IRoomGrain?> Room()
        => Task.FromResult(_room);

    public Task EnterRoom(IRoomGrain room)
    {
        _room = room;
        return Task.CompletedTask;
    }

    public async Task<GameCommandResult> Play(IAdventureWorldGrain world, GameCommand command)
    {
        var commandHandler = new GameCommandHandler(world, this);
        return await commandHandler.ExecuteAsync(command);
    }
}
