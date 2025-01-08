using System.Diagnostics;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : PassageOccupantGrainState
{
    public bool IsLoaded { get; set; }
}

public sealed class PlayerGrain(IGrainFactory factory, GameCommandExecuter commandExecutor)
    : PassageOccupantGrain<PlayerGrainState>, IPlayerGrain, IPassageEvents
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

        var pubSub = _factory.GetPassagePubSub();
        pubSub.Subscribe(this);

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

    protected override async Task OnPassageEnter(GameContext context, IPassageGrain passage)
    {
        var log = await Log();
        await log.AddLine(passage, this.GetPrimaryKeyString());
        await base.OnPassageEnter(context, passage);
    }

    public async Task OnPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        var thisPlayerKey = this.GetPrimaryKeyString();
        Debug.Assert(thisPlayerKey != occupantKey);

        var log = await Log();
        var passage = _factory.GetGrain<IPassageGrain>(passageKey);
        await log.UpdateLine(passage, thisPlayerKey);
    }

    public async Task OnPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        var thisPlayerKey = this.GetPrimaryKeyString();
        Debug.Assert(thisPlayerKey != occupantKey);

        var log = await Log();
        var passage = _factory.GetGrain<IPassageGrain>(passageKey);
        await log.UpdateLine(passage, thisPlayerKey);
    }

    private PlayerKey GetKey()
        => PlayerKey.Parse(this.GetPrimaryKeyString());
}
