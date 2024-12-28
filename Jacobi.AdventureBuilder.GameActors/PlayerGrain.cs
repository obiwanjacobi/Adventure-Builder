using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : AmInPassageGrainState
{
    public bool IsLoaded { get; set; }
}

public sealed class PlayerGrain(IGrainFactory factory)
    : AmInPassageGrain<PlayerGrainState>, IPlayerGrain
{
    private readonly IGrainFactory _factory = factory;

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

    public async Task<GameCommandResult> Play(IWorldGrain world, GameCommand command)
    {
        var log = await Log();
        await log.AddLine(command);

        var commandHandler = new GameCommandHandler(world, this);
        return await commandHandler.ExecuteAsync(command);
    }

    public Task<IPlayerLogGrain> Log()
    {
        var playerLogKey = GetKey().ToPlayerLogKey();
        return Task.FromResult(_factory.GetGrain<IPlayerLogGrain>(playerLogKey));
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
