using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameActors;

// NOTE: The passageKey passed in the NotifyPassageEnter/Exit calls is (typically?)
//  the same the grain's primary key. For now we leave it.

public sealed class PassageEventsGrain : Grain, IPassageEventsGrain
{
    internal const string StreamProviderName = "AzureQueueProvider";
    internal const string StreamNamespace = "passage-events";

    // TODO: move to grain state?
    private readonly IPassageEventsGrain _notifyClient;
    private readonly IStreamProvider _streamProvider;

    public PassageEventsGrain(IPassageEventsGrain notifyClient, ILogger<PassageEventsGrain> logger)
    {
        _notifyClient = notifyClient;
        _streamProvider = this.GetStreamProvider(StreamProviderName);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // TODO: state

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        var passageId = PassageKey.Parse(passageKey).PassageId;
        var stream = _streamProvider.GetStream<PassageEvent>(StreamNamespace, passageId);
        var passageEvent = new PassageEvent(PassageEventKind.Enter, passageKey);
        await stream.OnNextAsync(passageEvent);

        await _notifyClient.NotifyPassageEnter(context, passageKey, occupantKey);
    }

    public async Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        var passageId = PassageKey.Parse(passageKey).PassageId;
        var stream = _streamProvider.GetStream<PassageEvent>(StreamNamespace, passageId);
        var passageEvent = new PassageEvent(PassageEventKind.Exit, passageKey);
        await stream.OnNextAsync(passageEvent);

        await _notifyClient.NotifyPassageExit(context, passageKey, occupantKey);
    }
}

public static class PassageEventsExtensions
{
    public static IPassageEventsGrain GetPassagePubSub(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageEventsGrain>(passageKey);
    public static IPassageEventsGrain GetNotifyPassage(this IGrainFactory factory, string passageKey)
        => factory.GetGrain<IPassageEventsGrain>(passageKey);

    public static async Task<StreamSubscriptionHandle<PassageEvent>> SubscribePassageEvents(this Grain grain, string passageKey)
    {
        var subscriber = (IPassageEvents)grain;
        var streamProvider = grain.GetStreamProvider(PassageEventsGrain.StreamProviderName);
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
