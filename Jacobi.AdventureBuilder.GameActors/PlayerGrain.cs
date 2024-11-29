using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState
{
    public IPassageGrain? Room { get; set; }
}

public sealed class PlayerGrain : Grain<PlayerGrainState>, IPlayerGrain
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

    public Task<IPassageGrain?> Room()
        => Task.FromResult(this.State.Room);

    public Task EnterRoom(IPassageGrain room)
    {
        this.State.Room = room;
        return Task.CompletedTask;
    }

    public async Task<GameCommandResult> Play(IAdventureWorldGrain world, GameCommand command)
    {
        var commandHandler = new GameCommandHandler(world, this);
        return await commandHandler.ExecuteAsync(command);
    }
}
