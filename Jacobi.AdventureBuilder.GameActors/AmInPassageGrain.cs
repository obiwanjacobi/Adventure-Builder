using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public abstract class AmInPassageGrainState
{
    public IPassageGrain? Passage { get; set; }
}

public abstract class AmInPassageGrain<T> : Grain<T>, IAmInPassage
    where T : AmInPassageGrainState
{
    public abstract Task<string> Name();
    public abstract Task<string> Description();

    public async Task EnterPassage(IPassageGrain passage)
    {
        var key = this.GetPrimaryKeyString();
        if (State.Passage is not null)
        {
            await State.Passage.Exit(key);
            await OnPassageExit(State.Passage);
        }

        State.Passage = passage;
        await passage.Enter(key);
        await OnPassageEnter(passage);

        await WriteStateAsync();
    }

    protected virtual Task OnPassageEnter(IPassageGrain passage)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnPassageExit(IPassageGrain passage)
    {
        return Task.CompletedTask;
    }
}
