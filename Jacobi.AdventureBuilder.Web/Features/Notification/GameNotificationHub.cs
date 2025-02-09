using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

public interface IGameNotifications
{
    Task OnPassageExit(string occupantKey);
    Task OnPassageEnter(string occupantKey);
    Task OnPlayerLogChanged();
}

public sealed class GameNotificationHub : Hub<IGameNotifications>
{
    public Task Subscribe(string playerKey, string? passageKey, [FromServices] INotificationUsers notificationUsers)
    {
        return notificationUsers.Subscribe(Context.ConnectionId, playerKey, passageKey);
    }
}
