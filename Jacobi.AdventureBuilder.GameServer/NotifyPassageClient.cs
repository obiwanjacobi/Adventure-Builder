using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameServer;

public sealed class NotifyPassageClient : IPassageEventsGrain
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

internal static class NotifyExtensions
{
    public const string WebServerUrl_Setting = "services:webfrontend:https:0";

    public static IHttpClientBuilder AddNotifyPassage(this IHostApplicationBuilder builder)
    {
        var serverUrl = builder.Configuration[WebServerUrl_Setting]
            ?? throw new InvalidOperationException($"Missing Configuration '{WebServerUrl_Setting}'. Did you call the `WithReference(webFrontend)` extension method?");

        return builder.Services.AddNotifyPassageClient(serverUrl);
    }

    public static IHttpClientBuilder AddNotifyPassageClient(this IServiceCollection services, string serverUrl)
    {
        return services.AddHttpClient<IPassageEventsGrain, NotifyPassageClient>(client =>
        {
            client.BaseAddress = new Uri(serverUrl);
        });
    }
}
