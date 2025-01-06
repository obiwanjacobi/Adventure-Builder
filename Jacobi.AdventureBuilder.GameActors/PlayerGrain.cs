using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : PassageOccupantGrainState
{
    public bool IsLoaded { get; set; }
}

public sealed class PlayerGrain(IGrainFactory factory, GameCommandExecuter commandExecutor)
    : PassageOccupantGrain<PlayerGrainState>, IPlayerGrain
{
    private readonly IGrainFactory _factory = factory;
    private readonly GameCommandExecuter _commandExecutor = commandExecutor;

    public override Task<string> Name()
        => Task.FromResult(PlayerKey.Parse(this.GetPrimaryKeyString()).Nickname);
    public override Task<string> Description()
        => Task.FromResult("(Player)");

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
        }
        return base.OnActivateAsync(cancellationToken);
    }

    public async Task<GameCommandResult> Play(IWorldGrain world, IPassageGrain passage, GameCommand command)
    {
        var log = await Log();
        var isNavigationCmd = NavigationCommandHandler.IsNavigationCommand(command);

        if (isNavigationCmd)
            await log.AddLine(command);

        var result = await _commandExecutor.ExecuteAsync(world, this, passage, command);

        if (result.Success && !isNavigationCmd)
            await log.UpdateLine(passage, this.GetPrimaryKeyString(), command);

        return result;
    }

    public Task<IPlayerLogGrain> Log()
    {
        var playerLogKey = GetKey().ToPlayerLogKey();
        return Task.FromResult(_factory.GetGrain<IPlayerLogGrain>(playerLogKey));
    }

    public Task<IPlayerInventoryGrain> Inventory()
    {
        var playerInventoryKey = GetKey().ToPlayerInventoryKey();
        return Task.FromResult(_factory.GetGrain<IPlayerInventoryGrain>(playerInventoryKey));
    }

    protected override async Task OnPassageEnter(IPassageGrain passage)
    {
        var log = await Log();
        await log.AddLine(passage, this.GetPrimaryKeyString());
        await base.OnPassageEnter(passage);
    }

    private PlayerKey GetKey()
        => PlayerKey.Parse(this.GetPrimaryKeyString());
}
