using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jacobi.AdventureBuilder.ApiClient;

public static class Extensions
{
    public const string ApiServiceUrl_Setting = "services:apiservice:https:0";

    public static IHttpClientBuilder AddApiClient(this IHostApplicationBuilder builder)
    {
        var serverUrl = builder.Configuration[ApiServiceUrl_Setting]
            ?? throw new InvalidOperationException($"Missing Configuration '{ApiServiceUrl_Setting}'. Did you call the `WithReference(apiService)` extension method?");

        return builder.Services.AddApiClient(serverUrl);
    }

    public static IHttpClientBuilder AddApiClient(this IServiceCollection services, string serverUrl)
    {
        return services.AddHttpClient<IAdventureClient, AdventureClient>(client =>
        {
            client.BaseAddress = new Uri(serverUrl);
        });
    }
}
