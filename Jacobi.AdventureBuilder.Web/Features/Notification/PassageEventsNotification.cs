using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Streams;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

public class PassageEventsNotification : IPassageEvents
{
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public PassageEventsNotification(INotificationService notificationService, ILogger<PassageEventsNotification> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task OnNextAsync(PassageEvent item, StreamSequenceToken? token = null)
    {
        return item.Kind switch
        {
            PassageEventKind.Enter => _notificationService.NotifyPassageEnter(item.PassageKey, item.OccupantKey),
            PassageEventKind.Exit => _notificationService.NotifyPassageExit(item.PassageKey, item.OccupantKey),
            _ => Task.CompletedTask
        };
    }
    public Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }
    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogWarning(ex, "PassageEventNotification Error");
        return Task.CompletedTask;
    }
}
