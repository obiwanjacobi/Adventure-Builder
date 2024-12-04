using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState
{
    public bool IsLoaded { get; set; }
    public PlayerInfo? PlayerInfo { get; set; }
    public IPassageGrain? Passage { get; set; }
}

public sealed class PlayerGrain : Grain<PlayerGrainState>, IPlayerGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
            State.PlayerInfo = new(this.GetPrimaryKeyString(), Guid.Empty, "John Doe");
        }
        return base.OnActivateAsync(cancellationToken);
    }

    public Task<PlayerInfo> PlayerInfo()
        => Task.FromResult(State.PlayerInfo!);

    public Task SetPlayerInfo(Guid accountId, string nickname)
    {
        State.IsLoaded = true;
        State.PlayerInfo = new PlayerInfo(this.GetPrimaryKeyString(), accountId, nickname);
        return WriteStateAsync();
    }

    public Task<IPassageGrain?> Passage()
        => Task.FromResult(State.Passage);

    public Task EnterPassage(IPassageGrain passage)
    {
        State.Passage = passage;
        return WriteStateAsync();
    }

    public async Task<GameCommandResult> Play(IWorldGrain world, GameCommand command)
    {
        var commandHandler = new GameCommandHandler(world, this);
        return await commandHandler.ExecuteAsync(command);
    }
}
