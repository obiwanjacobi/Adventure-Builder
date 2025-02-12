using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : PassageOccupantGrainState
{
    public bool IsLoaded { get; set; }
}

[Reentrant]
public sealed class PlayerGrain(GameCommandExecuter commandExecutor)
    : PassageOccupantGrain<PlayerGrainState>, IPlayerGrain
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

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<GameCommandResult> Play(IWorldGrain world, IPassageGrain passage, GameCommand command)
    {
        var log = GrainFactory.GetPlayerLog(this);
        //var isNavigationCmd = NavigationCommandHandler.IsNavigationCommand(command);

        //if (isNavigationCmd)
        await log.AddLine(command);

        return await _commandExecutor.ExecuteCommand(world, this, passage, command);
    }

    protected override async Task OnPassageEnter(GameContext context, IPassageGrain passage)
    {
        await base.OnPassageEnter(context, passage);
    }

    protected override async Task OnPassageExit(GameContext context, IPassageGrain passage)
    {
        var log = GrainFactory.GetPlayerLog(this);
        await log.AddLine(passage, this.GetPrimaryKeyString());

        await base.OnPassageExit(context, passage);
    }
}
