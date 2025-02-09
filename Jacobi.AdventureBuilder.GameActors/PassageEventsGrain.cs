using System.Diagnostics;
using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameActors;

// NOTE: The passageKey passed in the NotifyPassageEnter/Exit calls is (typically?)
//  the same the grain's primary key. For now we leave it.

public sealed class PassageEventsGrain : EventsGrain, IPassageEventsGrain
{
    public const string StreamNamespace = "passage-events";

    private readonly INotifyPassage _notifyClient;

    public PassageEventsGrain(INotifyPassage notifyClient)
    {
        _notifyClient = notifyClient;
    }

    public async Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        Debug.Assert(passageKey == this.GetPrimaryKeyString());

        var passageEvent = new PassageEvent(PassageEventKind.Enter, passageKey, occupantKey);
        await SendEvent(passageEvent);
        await _notifyClient.NotifyPassageEnter(context, passageKey, occupantKey);
    }

    public async Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        Debug.Assert(passageKey == this.GetPrimaryKeyString());

        var passageEvent = new PassageEvent(PassageEventKind.Exit, passageKey, occupantKey);
        await SendEvent(passageEvent);
        await _notifyClient.NotifyPassageExit(context, passageKey, occupantKey);
    }

    private Task SendEvent(PassageEvent passageEvent)
    {
        var passageId = PassageKey.Parse(passageEvent.PassageKey).PassageId;
        var stream = StreamProvider.GetStream<PassageEvent>(PassageEventsGrain.StreamNamespace, passageId);
        return stream.OnNextAsync(passageEvent);
    }
}

public static class PassageEventsExtensions
{
    public static IPassageEventsGrain GetPassagePubSub(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageEventsGrain>(passageKey);
    public static INotifyPassage GetNotifyPassage(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageEventsGrain>(passageKey);

    public static async Task<StreamSubscriptionHandle<PassageEvent>> SubscribePassageEvents(this Grain grain, string passageKey)
    {
        var subscriber = (IPassageEvents)grain;
        var streamProvider = grain.GetStreamProvider(EventsGrain.StreamProviderName);
        var passageId = PassageKey.Parse(passageKey).PassageId;
        var streamId = StreamId.Create(PassageEventsGrain.StreamNamespace, passageId);
        var stream = streamProvider.GetStream<PassageEvent>(streamId);
        var subscription = await stream.SubscribeAsync(subscriber);
        return subscription;
    }
}

[GenerateSerializer, Immutable]
public sealed record class ClientNotifyEvent
{
    public ClientNotifyEvent(PassageEventKind kind, string passageKey)
    {
        Kind = kind;
        PassageKey = passageKey;
    }

    [Id(0)]
    public PassageEventKind Kind { get; }
    [Id(1)]
    public string PassageKey { get; }
}
