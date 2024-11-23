namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal static class AdventureExtensions
{
    public static IServiceCollection AddAdventureServices(this IServiceCollection services)
    {
        services.AddScoped<IAdventureRepository, AdventureRepository>();
        return services;
    }
}
