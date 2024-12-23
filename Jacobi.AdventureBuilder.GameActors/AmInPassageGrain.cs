using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public abstract class AmInPassageGrainState
{
    public Option<IPassageGrain> Passage { get; set; } = Option<IPassageGrain>.None;
    public long ExtraId { get; set; }
}

public abstract class AmInPassageGrain<T> : Grain<T>, IAmInPassage
    where T : AmInPassageGrainState
{
    protected abstract string Name { get; }
    protected abstract string Description { get; }

    public async Task EnterPassage(IPassageGrain passage, Option<INotifyPassage> notifyPassage)
    {
        State.Passage.IfSome(passage =>
        {
            passage.RemoveExtraInfo(State.ExtraId);
            notifyPassage.IfSome(notify =>
                notify.NotifyPassageExit(passage.GetPrimaryKeyString(), this.GetPrimaryKeyString()));
        });

        State.ExtraId = await passage.AddExtraInfo(new AdventureExtraInfo
        {
            Name = Name,
            Description = Description,
            PassageId = 0
        });

        State.Passage = Option<IPassageGrain>.Some(passage);
        await WriteStateAsync();

        notifyPassage.IfSome(notify =>
            notify.NotifyPassageEnter(passage.GetPrimaryKeyString(), this.GetPrimaryKeyString()));
    }
}
