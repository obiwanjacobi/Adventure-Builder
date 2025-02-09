using Microsoft.AspNetCore.SignalR;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

public interface INotificationService
{
    // player
    Task NotifyPlayerLogChanged(string playerKey);

    // passage
    Task NotifyPassageEnter(string passageKey, string occupantKey);
    Task NotifyPassageExit(string passageKey, string occupantKey);
}

public interface INotificationUsers
{
    Task Subscribe(string connectionId, string playerKey, string? passageKey);
}

internal sealed class NotificationService(IHubContext<GameNotificationHub, IGameNotifications> hubContext)
    : INotificationService, INotificationUsers
{
    private readonly Lock _lock = new();    // we are a singleton
    private readonly IHubContext<GameNotificationHub, IGameNotifications> _hubContext = hubContext;
    // maps player keys to user info
    private readonly Dictionary<string, UserNotificationInfo> _userMap = new();
    // maps passage keys to user info
    private readonly Dictionary<string, List<UserNotificationInfo>> _passageMap = new();

    public async Task NotifyPassageEnter(string passageKey, string occupantKey)
    {
        await Enter(passageKey, occupantKey);
        await _hubContext.Clients.Group(passageKey).OnPassageEnter(occupantKey);
    }

    public async Task NotifyPassageExit(string passageKey, string occupantKey)
    {
        await Exit(passageKey, occupantKey);
        await _hubContext.Clients.Group(passageKey).OnPassageExit(occupantKey);
    }

    public Task NotifyPlayerLogChanged(string playerKey)
    {
        UserNotificationInfo? userInfo;

        lock (_lock)
        {
            _userMap.TryGetValue(playerKey, out userInfo);
        }

        if (userInfo is not null)
            return _hubContext.Clients.Client(userInfo.ConnectionId).OnPlayerLogChanged();

        return Task.CompletedTask;
    }

    public async Task Subscribe(string connectionId, string playerKey, string? passageKey)
    {
        lock (_lock)
        {
            _userMap[playerKey] = new UserNotificationInfo(connectionId, playerKey)
            {
                PassageKey = String.IsNullOrWhiteSpace(passageKey) ? null : passageKey
            };
        }

        if (!String.IsNullOrWhiteSpace(passageKey))
            await Enter(passageKey, playerKey);
    }

    private async Task Enter(string passageKey, string occupantKey)
    {
        UserNotificationInfo? userInfo;

        lock (_lock)
        {
            if (_userMap.TryGetValue(occupantKey, out userInfo))
            {
                if (!_passageMap.TryGetValue(passageKey, out _))
                    _passageMap[passageKey] = [];

                userInfo.PassageKey = passageKey;
                _passageMap[passageKey].Add(userInfo);
            }
        }

        if (userInfo is not null)
            await _hubContext.Groups.AddToGroupAsync(userInfo.ConnectionId, passageKey);
    }

    private async Task Exit(string passageKey, string occupantKey)
    {
        UserNotificationInfo? userInfo;

        lock (_lock)
        {
            if (_userMap.TryGetValue(occupantKey, out userInfo) &&
                _passageMap.TryGetValue(passageKey, out var users))
            {
                users.Remove(userInfo);
                userInfo.PassageKey = null;
            }
        }

        if (userInfo is not null)
            await _hubContext.Groups.RemoveFromGroupAsync(userInfo.ConnectionId, passageKey);
    }

    // ------------------------------------------------------------------------

    private sealed class UserNotificationInfo(string connectionId, string playerKey)
    {
        public string ConnectionId { get; } = connectionId;
        public string PlayerKey { get; } = playerKey;
        public string? PassageKey { get; set; }
    }
}
