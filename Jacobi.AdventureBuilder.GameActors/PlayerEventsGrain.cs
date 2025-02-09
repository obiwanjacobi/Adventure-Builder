using System.Diagnostics;
using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerEventsGrain : EventsGrain, IPlayerEventsGrain
{
    public const string StreamNamespace = "player-events";

    private readonly INotifyPlayer _notifyClient;

    public PlayerEventsGrain(INotifyPlayer notifyClient)
    {
        _notifyClient = notifyClient;
    }

    public async Task NotifyPlayerLogChanged(string playerKey)
    {
        Debug.Assert(playerKey == this.GetPrimaryKeyString());

        var playerEvent = new PlayerEvent(PlayerEventKind.LogChanged, playerKey);
        await SendEvent(playerEvent);
        await _notifyClient.NotifyPlayerLogChanged(playerKey);
    }

    private Task SendEvent(PlayerEvent playerEvent)
    {
        var playerId = PlayerKey.Parse(playerEvent.PlayerKey).Nickname;
        var streamId = StreamId.Create(PlayerEventsGrain.StreamNamespace, playerId);
        var stream = StreamProvider.GetStream<PlayerEvent>(streamId);
        return stream.OnNextAsync(playerEvent);
    }
}

public static class PlayerEventsExtensions
{
    public static IPlayerEventsGrain GetPlayerEvents(this IGrainFactory factory, string playerKey)
        => factory.GetGrain<IPlayerEventsGrain>(playerKey);

    public static async Task<StreamSubscriptionHandle<PlayerEvent>> SubscribePlayerEvents(this Grain grain, string playerKey)
    {
        var subscriber = (IPlayerEvents)grain;
        var streamProvider = grain.GetStreamProvider(EventsGrain.StreamProviderName);
        var playerId = PlayerKey.Parse(playerKey).Nickname;
        var streamId = StreamId.Create(PlayerEventsGrain.StreamNamespace, playerId);
        var stream = streamProvider.GetStream<PlayerEvent>(streamId);
        var subscription = await stream.SubscribeAsync(subscriber);
        return subscription;
    }
}
