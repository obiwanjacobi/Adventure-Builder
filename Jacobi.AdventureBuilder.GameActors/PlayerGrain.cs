using System.Diagnostics;
using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : PassageOccupantGrainState
{
    public bool IsLoaded { get; set; }
}

public sealed class PlayerGrain(GameCommandExecuter commandExecutor)
    : PassageOccupantGrain<PlayerGrainState>, IPlayerGrain//, IPassageEvents
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
            await log.UpdateLine(passage, this.GetPrimaryKeyString(), command);

        return result;
    }

    protected override async Task OnPassageEnter(GameContext context, IPassageGrain passage)
    {
        await Subscribe();

        var log = GrainFactory.GetPlayerLog(this);
        await log.AddLine(passage, this.GetPrimaryKeyString());
        await base.OnPassageEnter(context, passage);
    }

    protected override async Task OnPassageExit(GameContext context, IPassageGrain passage)
    {
        await Unsubscribe();
        await base.OnPassageExit(context, passage);
    }

    public async Task OnPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        var playerKey = this.GetPrimaryKeyString();
        Debug.Assert(playerKey != occupantKey);

        var log = GrainFactory.GetPlayerLog(this);
        var passage = GrainFactory.GetGrain<IPassageGrain>(passageKey);
        await log.UpdateLine(passage, playerKey);
    }

    public async Task OnPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        var playerKey = this.GetPrimaryKeyString();
        Debug.Assert(playerKey != occupantKey);

        var log = GrainFactory.GetPlayerLog(this);
        var passage = GrainFactory.GetGrain<IPassageGrain>(passageKey);
        await log.UpdateLine(passage, playerKey);
    }

    //public Task OnNextAsync(PassageEvent passageEvent, StreamSequenceToken? token = null)
    //{
    //    return passageEvent.Action switch
    //    {
    //        PassageAction.Enter => OnPassageEnter(passageEvent.Context, passageEvent.PassageKey, passageEvent.OccupantKey),
    //        PassageAction.Exit => OnPassageExit(passageEvent.Context, passageEvent.PassageKey, passageEvent.OccupantKey),
    //        _ => Task.CompletedTask
    //    };
    //}
    //public Task OnCompletedAsync()
    //{
    //    return Task.CompletedTask;
    //}
    //public Task OnErrorAsync(Exception ex)
    //{
    //    Console.WriteLine(ex);
    //    return Task.CompletedTask;
    //}

    //private StreamSubscriptionHandle<PassageEvent>? _subHandle;
    private PlayerGrainEventHandler? _eventHandler;
    private Task Subscribe()
    {
        _eventHandler = new PlayerGrainEventHandler(GrainFactory, this.GetPrimaryKeyString());

        return State.Passage!.Subscribe(
            GrainFactory.CreateObjectReference<IPassageEvents>(_eventHandler),
            //(IPassageEvents)_eventHandler,
            this.GetPrimaryKeyString());

        //_subHandle = await this.Subscribe(this, State.Passage.GetPrimaryKeyString());
    }
    private Task Unsubscribe()
    {
        _eventHandler = null;
        return State.Passage!.Unsubscribe(this.GetPrimaryKeyString());

        //if (_subHandle is not null)
        //{
        //    await _subHandle.UnsubscribeAsync();
        //    _subHandle = null;
        //}
    }
}

public sealed class PlayerGrainEventHandler : IPassageEvents
{
    private readonly IGrainFactory _factory;
    private readonly string _playerKey;

    public PlayerGrainEventHandler(IGrainFactory factory, string playerKey)
    {
        _factory = factory;
        _playerKey = playerKey;
    }

    public Task OnPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        Debug.Assert(_playerKey != occupantKey);
        var log = _factory.GetGrain<IPlayerLogGrain>(_playerKey);
        var passage = _factory.GetGrain<IPassageGrain>(passageKey);
        return log.UpdateLine(passage, _playerKey);
    }

    public Task OnPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        Debug.Assert(_playerKey != occupantKey);
        var log = _factory.GetGrain<IPlayerLogGrain>(_playerKey);
        var passage = _factory.GetGrain<IPassageGrain>(passageKey);
        return log.UpdateLine(passage, _playerKey);
    }
}
