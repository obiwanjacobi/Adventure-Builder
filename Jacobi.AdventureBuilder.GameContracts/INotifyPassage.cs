namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPassage
{
    Task NotifyPassageEnter(string passageKey, string characterKey);
    Task NotifyPassageExit(string passageKey, string characterKey);
}

public interface IPassageObserverProviderGrain : INotifyPassage, IGrainWithGuidKey
{
    Task Subscribe(IPassageObserver subscriber);
    Task Unsubscribe(IPassageObserver subscriber);
}

public interface IPassageObserver : IGrainObserver
{
    Task OnPassageEnter(string passageKey, string characterKey);
    Task OnPassageExit(string passageKey, string characterKey);
}
