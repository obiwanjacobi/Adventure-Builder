using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerGrainState : AmInPassageGrainState
{
    public bool IsLoaded { get; set; }
}

public sealed class PlayerGrain(INotifyPassage notifyPassage)
    : AmInPassageGrain<PlayerGrainState>, IPlayerGrain
{
    private readonly INotifyPassage _notifyPassage = notifyPassage;

    protected override string Name
        => PlayerKey.Parse(this.GetPrimaryKeyString()).Nickname;
    protected override string Description
        => "(Player)";

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
        var commandHandler = new GameCommandHandler(world, this, _notifyPassage);
        return await commandHandler.ExecuteAsync(command);
    }
}
