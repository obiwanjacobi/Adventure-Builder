namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPassage
{
    Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey);
    Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey);
}

public interface IPassageEventsProviderGrain : INotifyPassage, IGrainWithStringKey
{
    Task Subscribe(IPassageEvents subscriber);
    Task Unsubscribe(IPassageEvents subscriber);
}

public interface IPassageEvents : IGrainWithStringKey
{
    Task OnPassageEnter(GameContext context, string passageKey, string occupantKey);
    Task OnPassageExit(GameContext context, string passageKey, string occupantKey);
}
