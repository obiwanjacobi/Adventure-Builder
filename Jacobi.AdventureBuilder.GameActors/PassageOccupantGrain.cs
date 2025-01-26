using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public abstract class PassageOccupantGrainState
{
    public IPassageGrain? Passage { get; set; }
}

public abstract class PassageOccupantGrain<T> : Grain<T>, IPassageOccupantGrain
    where T : PassageOccupantGrainState
{
    public abstract Task<string> Name();
    public abstract Task<string> Description();

    public async Task GotoPassage(GameContext context, IPassageGrain passage)
    {
        var key = this.GetPrimaryKeyString();
        if (State.Passage is not null)
        {
            await State.Passage.Exit(context, key);
            await OnPassageExit(context, State.Passage);
        }

        State.Passage = passage;
        await passage.Enter(context, key);
        await OnPassageEnter(context, passage);

        await WriteStateAsync();
    }

    protected virtual Task OnPassageEnter(GameContext context, IPassageGrain passage)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnPassageExit(GameContext context, IPassageGrain passage)
    {
        return Task.CompletedTask;
    }

    protected IPassageEventsProviderGrain GetPassagePubSub()
        => GrainFactory.GetPassagePubSub(State.Passage.GetPrimaryKeyString());
}
