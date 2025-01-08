using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageObserverProviderGrain : Grain, IPassageObserverProviderGrain
{
    private readonly IGrainFactory _factory;
    private readonly INotifyPassage _notifyPassage;
    private readonly ObserverManager<PassageObserver> _subManager;
    private readonly List<IPassageEvents> _subscribers = [];
    private readonly IGrainTimer _timer;
    private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);

    public PassageObserverProviderGrain(IGrainFactory factory, INotifyPassage notifyPassage, ILogger<PassageObserverProviderGrain> logger)
    {
        _factory = factory;
        _notifyPassage = notifyPassage;
        _subManager = new ObserverManager<PassageObserver>(_timeout, logger);
        _timer = this.RegisterGrainTimer(RefreshSubscriptions, _timeout, _timeout);
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // TODO: state
        return base.OnActivateAsync(cancellationToken);
    }

    public Task Subscribe(IPassageEvents subscriber)
    {
        var stub = new PassageObserver(subscriber);
        _subscribers.Add(subscriber);
        _subManager.Subscribe(stub, stub);
        return Task.CompletedTask;
    }

    private Task RefreshSubscriptions(CancellationToken ct)
    {
        foreach (var sub in _subscribers)
        {
            var stub = new PassageObserver(sub);
            _subManager.Subscribe(stub, stub);
        }

        return Task.CompletedTask;
    }

    public Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        _subManager.Notify(sub =>
        {
            if (sub.PrimaryKey != occupantKey)
                sub.OnPassageEnter(context, passageKey, occupantKey);
        });
        return _notifyPassage.NotifyPassageEnter(context, passageKey, occupantKey);
    }

    public Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        _subManager.Notify(sub =>
        {
            if (sub.PrimaryKey != occupantKey)
                sub.OnPassageExit(context, passageKey, occupantKey);
        });
        return _notifyPassage.NotifyPassageExit(context, passageKey, occupantKey);
    }

    //-------------------------------------------------------------------------

    private sealed class PassageObserver : IPassageObserver
    {
        private readonly IPassageEvents _target;

        public PassageObserver(IPassageEvents target)
        {
            _target = target;
        }

        public string PrimaryKey
            => _target.GetPrimaryKeyString();

        public Task OnPassageEnter(GameContext context, string passageKey, string occupantKey)
        {
            return _target.OnPassageEnter(context, passageKey, occupantKey);
        }

        public Task OnPassageExit(GameContext context, string passageKey, string occupantKey)
        {
            return _target.OnPassageExit(context, passageKey, occupantKey);
        }
    }
}

public static class PassageObserverExtensions
{
    public static IPassageObserverProviderGrain GetPassagePubSub(this IGrainFactory factory)
        => factory.GetSingleton<IPassageObserverProviderGrain>();
    public static INotifyPassage GetNotifyPassage(this IGrainFactory factory)
        => factory.GetSingleton<IPassageObserverProviderGrain>();
}

public interface IPassageObserver : IPassageEvents, IGrainObserver;
