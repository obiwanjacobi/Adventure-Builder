using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Utilities;

namespace Jacobi.AdventureBuilder.GameActors;

// NOTE: The passageKey passed in the NotifyPassageEnter/Exit calls is (typically?)
//  the same the grain's primary key. For now we leave it.

public sealed class PassageObserverProviderGrain : Grain, IPassageObserverProviderGrain
{
    // TODO: move to grain state?
    private readonly INotifyPassage _notifyPassage;
    private readonly ObserverManager<PassageObserver> _subManager;
    private readonly List<PassageObserver> _subscribers = [];
    private readonly Queue<PassageEvent> _events = new();
    private readonly IGrainTimer _timer;
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);

    public PassageObserverProviderGrain(INotifyPassage notifyPassage, ILogger<PassageObserverProviderGrain> logger)
    {
        _notifyPassage = notifyPassage;
        _subManager = new ObserverManager<PassageObserver>(_timeout, logger);
        _timer = this.RegisterGrainTimer(OnTimer, _timeout, _timeout);
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // TODO: state
        return base.OnActivateAsync(cancellationToken);
    }

    public Task Subscribe(IPassageEvents subscriber)
    {
        if (FindObserver(subscriber.GetPrimaryKeyString()) is null)
        {
            var stub = new PassageObserver(subscriber);
            _subscribers.Add(stub);
            _subManager.Subscribe(stub, stub);
        }
        return Task.CompletedTask;
    }

    public Task Unsubscribe(IPassageEvents subscriber)
    {
        var stub = FindObserver(subscriber.GetPrimaryKeyString());
        if (stub is not null)
            _subManager.Unsubscribe(stub);
        return Task.CompletedTask;
    }

    private PassageObserver? FindObserver(string primaryKey)
    {
        var stub = _subscribers.SingleOrDefault(sub => sub.PrimaryKey == primaryKey);
        return stub;
    }

    private async Task OnTimer(CancellationToken ct)
    {
        while (_events.Count > 0)
        {
            var evnt = _events.Dequeue();
            switch (evnt.Kind)
            {
                case PassageEventKind.PassageEnter:
                    await OnNotifyPassageEnter(evnt.Context, evnt.PassageKey, evnt.OccupantKey);
                    break;
                case PassageEventKind.PassageExit:
                    await OnNotifyPassageExit(evnt.Context, evnt.PassageKey, evnt.OccupantKey);
                    break;
            }
        }
        await RefreshSubscriptions(ct);
    }

    private Task RefreshSubscriptions(CancellationToken ct)
    {
        foreach (var stub in _subscribers)
        {
            _subManager.Subscribe(stub, stub);
        }

        return Task.CompletedTask;
    }

    public Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        //_events.Enqueue(new(PassageEventKind.PassageEnter, context, passageKey, occupantKey));
        //return Task.CompletedTask;
        return OnNotifyPassageEnter(context, passageKey, occupantKey);
    }

    public Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        //_events.Enqueue(new(PassageEventKind.PassageExit, context, passageKey, occupantKey));
        //return Task.CompletedTask;
        return OnNotifyPassageExit(context, passageKey, occupantKey);
    }

    private Task OnNotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        _subManager.Notify(sub =>
        {
            if (sub.PrimaryKey != occupantKey)
                sub.OnPassageEnter(context, passageKey, occupantKey);
        });
        return _notifyPassage.NotifyPassageEnter(context, passageKey, occupantKey);
    }

    private Task OnNotifyPassageExit(GameContext context, string passageKey, string occupantKey)
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

    //-------------------------------------------------------------------------

    private enum PassageEventKind { None, PassageEnter, PassageExit }
    private sealed record class PassageEvent(PassageEventKind Kind, GameContext Context, string PassageKey, string OccupantKey);
}

public static class PassageObserverExtensions
{
    public static IPassageObserverProviderGrain GetPassagePubSub(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageObserverProviderGrain>(passageKey);
    public static INotifyPassage GetNotifyPassage(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageObserverProviderGrain>(passageKey);
}

public interface IPassageObserver : IGrainObserver
{
    [OneWay]
    Task OnPassageEnter(GameContext context, string passageKey, string occupantKey);
    [OneWay]
    Task OnPassageExit(GameContext context, string passageKey, string occupantKey);
}
