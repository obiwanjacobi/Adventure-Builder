using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : PassageOccupantGrainState
{
    public bool IsLoaded { get; set; }
}

public sealed class PlayerGrain(GameCommandExecuter commandExecutor)
    : PassageOccupantGrain<PlayerGrainState>, IPlayerGrain, IPassageEvents
{
    private readonly GameCommandExecuter _commandExecutor = commandExecutor;

    public override Task<string> Name()
        => Task.FromResult(PlayerKey.Parse(this.GetPrimaryKeyString()).Nickname);
    public override Task<string> Description()
        => Task.FromResult("(Player)");

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
        }
        else if (State.Passage is not null)
        {
            await Subscribe();
        }

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<GameCommandResult> Play(IWorldGrain world, IPassageGrain passage, GameCommand command)
    {
        var log = GrainFactory.GetPlayerLog(this);
        var isNavigationCmd = NavigationCommandHandler.IsNavigationCommand(command);

        if (isNavigationCmd)
            await log.AddLine(command);

        var result = await _commandExecutor.ExecuteCommand(world, this, passage, command);

        if (result.Success && !isNavigationCmd)
            await log.UpdateLine(passage, command);

        return result;
    }

    protected override async Task OnPassageEnter(GameContext context, IPassageGrain passage)
    {
        await Subscribe();

        var log = GrainFactory.GetPlayerLog(this);
        await log.AddLine(passage);
        await base.OnPassageEnter(context, passage);
    }

    protected override async Task OnPassageExit(GameContext context, IPassageGrain passage)
    {
        await Unsubscribe();
        await base.OnPassageExit(context, passage);
    }

    public Task OnPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        if (this.GetPrimaryKeyString() != occupantKey)
            return UpdateLog(passageKey);

        return Task.CompletedTask;
    }

    public Task OnPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        if (this.GetPrimaryKeyString() != occupantKey)
            return UpdateLog(passageKey);

        return Task.CompletedTask;
    }

    private Task UpdateLog(string passageKey)
    {
        var log = GrainFactory.GetPlayerLog(this.GetPrimaryKeyString());
        var passage = GrainFactory.GetGrain<IPassageGrain>(passageKey);
        return log.UpdateLine(passage);
    }

    private Task Subscribe()
        => State.Passage!.Subscribe(this.Cast<IPassageEvents>(), this.GetPrimaryKeyString());

    private Task Unsubscribe()
        => State.Passage!.Unsubscribe(this.GetPrimaryKeyString());
}
