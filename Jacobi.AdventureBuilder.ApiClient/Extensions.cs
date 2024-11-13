using Microsoft.Extensions.DependencyInjection;

namespace Jacobi.AdventureBuilder.ApiClient;

public static class Extensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddSingleton<IAdventureClient, AdventureClient>();
        return services;
    }
}
