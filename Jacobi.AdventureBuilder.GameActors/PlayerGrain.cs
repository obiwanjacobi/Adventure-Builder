using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState
{
    public bool IsLoaded { get; set; }
    public PlayerInfo? PlayerInfo { get; set; }
    public IPassageGrain? Passage { get; set; }
    public long ExtraId { get; set; }
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

    public async Task EnterPassage(IPassageGrain passage)
    {
        ThrowIfNotLoaded();

        if (State.Passage is not null)
        {
            await State.Passage.RemoveExtraInfo(State.ExtraId);
        }

        State.ExtraId = await passage.AddExtraInfo(new AdventureExtraInfo
        {
            Name = State.PlayerInfo!.Nickname,
            Description = "(Player)",
            PassageId = 0
        });

        State.Passage = passage;
        await WriteStateAsync();
    }

    public async Task<GameCommandResult> Play(IWorldGrain world, GameCommand command)
    {
        ThrowIfNotLoaded();
        var commandHandler = new GameCommandHandler(world, this);
        return await commandHandler.ExecuteAsync(command);
    }

    private void ThrowIfNotLoaded()
    {
        if (!State.IsLoaded)
            throw new InvalidOperationException("This AdventureWorld has not been loaded.");
    }
}
