using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

public class PlayerEventsNotification : IPlayerEvents
{
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public PlayerEventsNotification(INotificationService notificationService, ILogger<PassageEventsNotification> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task OnNextAsync(PlayerEvent item, StreamSequenceToken? token = null)
    {
        return item.Kind switch
        {
            PlayerEventKind.LogChanged => _notificationService.NotifyPlayerLogChanged(item.PlayerKey),
            _ => Task.CompletedTask
        };
    }
    public Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }
    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogWarning(ex, "PlayerEventNotification Error");
        return Task.CompletedTask;
    }
}
