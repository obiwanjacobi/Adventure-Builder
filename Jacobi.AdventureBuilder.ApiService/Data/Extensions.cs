namespace Jacobi.AdventureBuilder.ApiService.Data;

internal static class DataExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddSingleton<IDatabase, CosmosDatabase>();
        return services;
    }
}
