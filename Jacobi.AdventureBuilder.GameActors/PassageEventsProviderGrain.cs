using System.Diagnostics;
using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameActors;

// NOTE: The passageKey passed in the NotifyPassageEnter/Exit calls is (typically?)
//  the same the grain's primary key. For now we leave it.

public sealed class PassageEventsProviderGrain : Grain, IPassageEventsProviderGrain
{
    private const string PassageEvents = "PassageEvents";

    // TODO: move to grain state?
    private readonly INotifyPassage _notifyPassage;
    private readonly Dictionary<string, StreamSubscriptionHandle<PassageEvent>> _subscribers = [];
    //private readonly IStreamProvider _streamProvider;

    public PassageEventsProviderGrain(INotifyPassage notifyPassage, ILogger<PassageEventsProviderGrain> logger)
    {
        _notifyPassage = notifyPassage;
        //_streamProvider = this.GetStreamProvider("AzureQueueProvider");
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // TODO: state
        return base.OnActivateAsync(cancellationToken);
    }

    public async Task Subscribe(IAsyncObserver<PassageEvent> subscriber, string subscriberKey)
    {
        //var stream = GetEventStream();
        //var subHandle = await stream.SubscribeAsync(subscriber);

        //_subscribers.Add(subscriberKey, subHandle);
    }

    public Task Unsubscribe(string subscriberKey)
    {
        if (_subscribers.TryGetValue(subscriberKey, out var subHandle))
        {
            _subscribers.Remove(subscriberKey);
            return subHandle.UnsubscribeAsync();
        }

        return Task.CompletedTask;
    }

    public async Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        var key = this.GetPrimaryKeyString();
        Debug.Assert(key == passageKey);

        //var stream = GetEventStream();
        //await stream.OnNextAsync(new PassageEvent(PassageAction.Enter, context, key, occupantKey));
        await _notifyPassage.NotifyPassageEnter(context, passageKey, occupantKey);
    }

    public async Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        var key = this.GetPrimaryKeyString();
        Debug.Assert(key == passageKey);

        //var stream = GetEventStream();
        //await stream.OnNextAsync(new PassageEvent(PassageAction.Exit, context, key, occupantKey));
        await _notifyPassage.NotifyPassageExit(context, passageKey, occupantKey);
    }

    //private IAsyncStream<PassageEvent> GetEventStream()
    //    => _streamProvider.GetStream<PassageEvent>(PassageEvents, this.GetPrimaryKeyString());
}

public static class PassageEventsExtensions
{
    public static IPassageEventsProviderGrain GetPassagePubSub(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageEventsProviderGrain>(passageKey);
    public static INotifyPassage GetNotifyPassage(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageEventsProviderGrain>(passageKey);

    public static async Task<StreamSubscriptionHandle<PassageEvent>> Subscribe(this Grain grain, IAsyncObserver<PassageEvent> subscriber, string passageKey)
    {
        var provider = grain.GetStreamProvider("AzureQueueProvider");
        var stream = provider.GetStream<PassageEvent>("PassageEvents", passageKey);
        var subHandle = await stream.SubscribeAsync(subscriber);
        return subHandle;
    }
}
