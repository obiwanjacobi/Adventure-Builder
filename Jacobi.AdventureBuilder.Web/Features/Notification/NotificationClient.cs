using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

internal sealed class NotificationClient : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;

    public NotificationClient(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/notifications"))
            .Build();
    }

    public Func<string, Task>? OnPassageEnter
    {
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _hubConnection.On<string>("OnPassageEnter", value);
        }
    }

    public Func<string, Task>? OnPassageExit
    {
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _hubConnection.On<string>("OnPassageExit", value);
        }
    }

    public async Task StartAsync(string playerKey, string? passageKey)
    {
        await _hubConnection.StartAsync();
        await _hubConnection.SendAsync("subscribe", playerKey, passageKey);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            GC.SuppressFinalize(this);
            await _hubConnection.DisposeAsync();
        }
    }
}
