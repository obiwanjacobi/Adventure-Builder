﻿namespace Jacobi.AdventureBuilder.Web.Features.Notification;

internal static class NotificationExtensions
{
    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddSignalR();

        // notification service
        services.AddSingleton<NotificationService>();
        services.AddSingleton<INotificationService>(serviceProvider
            => serviceProvider.GetRequiredService<NotificationService>());
        services.AddSingleton<INotificationUsers>(serviceProvider
            => serviceProvider.GetRequiredService<NotificationService>());

        //services.AddSingleton<IPassageEvents, PassageEventsNotification>();
        //services.AddSingleton<IPlayerEvents, PlayerEventsNotification>();

        return services;
    }

    public static void MapNotifications(this WebApplication app)
    {
        app.MapHub<GameNotificationHub>("/notifications");
    }
}
