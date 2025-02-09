using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPlayer
{
    Task NotifyPlayerLogChanged(string playerKey);
}

public interface IPlayerEventsGrain : INotifyPlayer, IGrainWithStringKey;

public interface IPlayerEvents : IAsyncObserver<PlayerEvent>;

public enum PlayerEventKind { None, LogChanged }

[GenerateSerializer, Immutable]
public sealed record class PlayerEvent
{
    public PlayerEvent(PlayerEventKind kind, string playerKey)
    {
        Kind = kind;
        PlayerKey = playerKey;
    }

    [Id(0)]
    public PlayerEventKind Kind { get; }
    [Id(1)]
    public string PlayerKey { get; }
}
