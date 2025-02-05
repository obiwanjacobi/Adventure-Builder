using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Streams;

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

    public async Task OnNextAsync(PassageEvent item, StreamSequenceToken? token = null)
    {
        var playerKey = this.GetPrimaryKeyString();
        var log = GrainFactory.GetPlayerLog(this);
        var passage = GrainFactory.GetGrain<IPassageGrain>(item.PassageKey);
        await log.UpdateLine(passage, playerKey);
    }

    public Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }
    public Task OnErrorAsync(Exception ex)
    {
        return Task.CompletedTask;
    }

    private StreamSubscriptionHandle<PassageEvent>? _subscription;

    private async Task Subscribe()
    {
        if (_subscription is not null) await Unsubscribe();
        if (State.Passage is null) return;

        _subscription = await this.SubscribePassageEvents(State.Passage.GetPrimaryKeyString());

        //var passageId = PassageKey.Parse(State.Passage.GetPrimaryKeyString()).PassageId;
        //var streamProvider = this.GetStreamProvider("AzureQueueProvider");
        //var streamId = StreamId.Create("passage-events", passageId);
        //var stream = streamProvider.GetStream<PassageEvent>(streamId);
        //_subscription = await stream.SubscribeAsync(this);
    }
    private async Task Unsubscribe()
    {
        if (_subscription is null) return;
        await _subscription.UnsubscribeAsync();
        _subscription = null;
    }

    //private IPassageEventsGrain GetPassagePubSub()
    //    => GrainFactory.GetPassagePubSub(State.Passage.GetPrimaryKeyString());
}
