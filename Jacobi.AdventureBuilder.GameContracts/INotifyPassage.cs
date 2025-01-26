using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPassage
{
    Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey);
    Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey);
}

public interface IPassageEvents : IGrainObserver
{
    [OneWay]
    Task OnPassageEnter(GameContext context, string passageKey, string occupantKey);
    [OneWay]
    Task OnPassageExit(GameContext context, string passageKey, string occupantKey);
}
