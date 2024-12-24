using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public abstract class AmInPassageGrainState
{
    public Option<IPassageGrain> Passage { get; set; } = Option<IPassageGrain>.None;
}

public abstract class AmInPassageGrain<T> : Grain<T>, IAmInPassage
    where T : AmInPassageGrainState
{
    public abstract Task<string> Name();
    public abstract Task<string> Description();

    public async Task EnterPassage(IPassageGrain passage, Option<INotifyPassage> notifyPassage)
    {
        var key = this.GetPrimaryKeyString();
        State.Passage.IfSome(currentPassage =>
        {
            currentPassage.Exit(key);
            notifyPassage.IfSome(notify =>
                    notify.NotifyPassageExit(currentPassage.GetPrimaryKeyString(), key));
        });

        await passage.Enter(key);
        State.Passage = Option<IPassageGrain>.Some(passage);
        await WriteStateAsync();

        notifyPassage.IfSome(notify =>
            notify.NotifyPassageEnter(passage.GetPrimaryKeyString(), key));
    }
}
