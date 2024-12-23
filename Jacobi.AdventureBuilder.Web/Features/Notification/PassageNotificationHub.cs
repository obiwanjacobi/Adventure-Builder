using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

public interface IPassageNotifications
{
    // characterKey is a Player or a NPC key.
    Task OnPassageExit(string characterKey);
    Task OnPassageEnter(string characterKey);
}

public sealed class PassageNotificationHub : Hub<IPassageNotifications>
{
    public Task Subscribe(string playerKey, string? passageKey, [FromServices] INotificationUsers notificationUsers)
    {
        return notificationUsers.Subscribe(Context.ConnectionId, playerKey, passageKey);
    }
}
