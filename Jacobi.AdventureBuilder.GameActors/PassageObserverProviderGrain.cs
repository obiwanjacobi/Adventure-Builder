using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageObserverProviderGrain : Grain, IPassageObserverProviderGrain
{
    private readonly INotifyPassage _notifyPassage;
    private readonly ObserverManager<IPassageObserver> _subManager;
    private readonly Dictionary<string, IPassageObserver> _subscribers = [];
    private readonly IGrainTimer _timer;
    private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);

    public PassageObserverProviderGrain(INotifyPassage notifyPassage, ILogger<PassageObserverProviderGrain> logger)
    {
        _notifyPassage = notifyPassage;
        _subManager = new ObserverManager<IPassageObserver>(_timeout, logger);
        _timer = this.RegisterGrainTimer(RefreshSubscriptions, _timeout, _timeout);
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        return base.OnActivateAsync(cancellationToken);
    }

    public Task Subscribe(IPassageObserver subscriber)
    {
        _subscribers.Add(subscriber.GetPrimaryKeyString(), subscriber);
        _subManager.Subscribe(subscriber, subscriber);
        return Task.CompletedTask;
    }

    public Task Unsubscribe(IPassageObserver subscriber)
    {
        _subscribers.Remove(subscriber.GetPrimaryKeyString());
        _subManager.Unsubscribe(subscriber);
        return Task.CompletedTask;
    }

    private Task RefreshSubscriptions(CancellationToken ct)
    {
        foreach (var sub in _subscribers)
        {
            _subManager.Subscribe(sub.Value, sub.Value);
        }

        return Task.CompletedTask;
    }

    public Task NotifyPassageEnter(string passageKey, string characterKey)
    {
        _subManager.Notify(sub => sub.OnPassageEnter(passageKey, characterKey));
        return _notifyPassage.NotifyPassageEnter(passageKey, characterKey);
    }

    public Task NotifyPassageExit(string passageKey, string characterKey)
    {
        _subManager.Notify(sub => sub.OnPassageExit(passageKey, characterKey));
        return _notifyPassage.NotifyPassageExit(passageKey, characterKey);
    }
}

public static class PassageObserverExtensions
{
    public static IPassageObserverProviderGrain GetPassagePubSub(this IGrainFactory factory)
        => factory.GetSingleton<IPassageObserverProviderGrain>();
    public static INotifyPassage GetNotifyPassage(this IGrainFactory factory)
        => factory.GetSingleton<IPassageObserverProviderGrain>();
}
