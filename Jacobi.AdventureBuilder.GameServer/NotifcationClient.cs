using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameServer;

public sealed class NotifyPassageClient : INotifyPassage
{
    private readonly HttpClient _httpClient;

    public NotifyPassageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task NotifyPassageEnter(GameContext context, string passageKey, string occupantKey)
    {
        var request = new { passageKey, occupantKey };
        return _httpClient.PostAsJsonAsync("/notify/passage/enter", request);
    }

    public Task NotifyPassageExit(GameContext context, string passageKey, string occupantKey)
    {
        var request = new { passageKey, occupantKey };
        return _httpClient.PostAsJsonAsync("/notify/passage/exit", request);
    }
}

public sealed class NotifyPlayerClient : INotifyPlayer
{
    private readonly HttpClient _httpClient;

    public NotifyPlayerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task NotifyPlayerLogChanged(string playerKey)
    {
        var request = new { playerKey };
        return _httpClient.PostAsJsonAsync("/notify/player/logchanged", request);
    }
}

internal static class NotifyExtensions
{
    public static IHttpClientBuilder AddNotifyPlayer(this IHostApplicationBuilder builder)
    {
        var serverUrl = GetWebServerUrl(builder);
        return builder.Services.AddNotifyPlayerClient(serverUrl);
    }

    public static IHttpClientBuilder AddNotifyPlayerClient(this IServiceCollection services, string serverUrl)
    {
        return services.AddHttpClient<INotifyPlayer, NotifyPlayerClient>(client =>
        {
            client.BaseAddress = new Uri(serverUrl);
        });
    }

    public static IHttpClientBuilder AddNotifyPassage(this IHostApplicationBuilder builder)
    {
        var serverUrl = GetWebServerUrl(builder);
        return builder.Services.AddNotifyPassageClient(serverUrl);
    }

    public static IHttpClientBuilder AddNotifyPassageClient(this IServiceCollection services, string serverUrl)
    {
        return services.AddHttpClient<INotifyPassage, NotifyPassageClient>(client =>
        {
            client.BaseAddress = new Uri(serverUrl);
        });
    }

    private const string WebServerUrl_Setting = "services:webfrontend:https:0";

    private static string GetWebServerUrl(IHostApplicationBuilder builder)
    {
        return builder.Configuration[WebServerUrl_Setting]
            ?? throw new InvalidOperationException($"Missing Configuration '{WebServerUrl_Setting}'. Did you call the `WithReference(webFrontend)` extension method?");
    }
}
