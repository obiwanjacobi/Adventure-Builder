using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jacobi.AdventureBuilder.GameClient;

public static class Extensions
{
    public static IHostApplicationBuilder AddGameClient(this IHostApplicationBuilder builder)
    {
        builder.AddKeyedAzureTableClient("game-clusters");
        builder.UseOrleansClient();

        builder.Services.AddSingleton<AdventureGameClient>();

        return builder;
    }
}
