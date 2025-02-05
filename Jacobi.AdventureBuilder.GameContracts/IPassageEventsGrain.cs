using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPassage
{
    Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey);
    Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey);
}

public interface IPassageEventsGrain : INotifyPassage, IGrainWithStringKey;

public interface IPassageEvents : IAsyncObserver<PassageEvent>;

public enum PassageEventKind { None, Enter, Exit }

[GenerateSerializer, Immutable]
public sealed record class PassageEvent
{
    public PassageEvent(PassageEventKind kind, string passageKey)
    {
        Kind = kind;
        PassageKey = passageKey;
    }

    [Id(0)]
    public PassageEventKind Kind { get; }
    [Id(1)]
    public string PassageKey { get; }
}
