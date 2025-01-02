using Microsoft.Extensions.DependencyInjection;

namespace Jacobi.AdventureBuilder.GameActors;

public static class Extensions
{
    public static IServiceCollection AddGrainServices(this IServiceCollection services)
    {
        // command handlers
        services.AddScoped<IGameCommandHandler, NavigationCommandHandler>();
        // depends on command handlers
        services.AddScoped<GameCommandExecuter>();

        return services;
    }
}
