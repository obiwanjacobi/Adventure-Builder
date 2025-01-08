using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

public interface IPassageNotifications
{
    Task OnPassageExit(string occupantKey);
    Task OnPassageEnter(string occupantKey);
}

public sealed class PassageNotificationHub : Hub<IPassageNotifications>
{
    public Task Subscribe(string playerKey, string? passageKey, [FromServices] INotificationUsers notificationUsers)
    {
        return notificationUsers.Subscribe(Context.ConnectionId, playerKey, passageKey);
    }
}
