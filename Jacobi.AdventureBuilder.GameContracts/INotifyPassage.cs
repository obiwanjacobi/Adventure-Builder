using Orleans.Concurrency;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPassage
{
    Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey);
    Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey);
}

public interface IPassageEvents : IGrainObserver//, IGrainWithStringKey
{
    [OneWay]
    Task OnPassageEnter(GameContext context, string passageKey, string occupantKey);
    [OneWay]
    Task OnPassageExit(GameContext context, string passageKey, string occupantKey);
}

public interface IPassageEventsProviderGrain : INotifyPassage, IGrainWithStringKey
{
    Task Subscribe(IAsyncObserver<PassageEvent> subscriber, string subscriberKey);
    Task Unsubscribe(string subscriberKey);
}

public enum PassageAction
{
    None,
    Enter,
    Exit
}

[GenerateSerializer, Immutable]
public sealed record class PassageEvent(
    PassageAction Action, GameContext Context, string PassageKey, string OccupantKey);
