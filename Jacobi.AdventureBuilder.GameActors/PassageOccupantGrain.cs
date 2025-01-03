using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public abstract class PassageOccupantGrainState
{
    public IPassageGrain? Passage { get; set; }
}

public abstract class PassageOccupantGrain<T> : Grain<T>, IPassageOccupant
    where T : PassageOccupantGrainState
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
