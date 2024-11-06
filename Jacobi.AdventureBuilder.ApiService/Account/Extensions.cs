namespace Jacobi.AdventureBuilder.ApiService.Account;

internal static class AccountExtensions
{
    public static IServiceCollection AddAccountServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        return services;
    }
}
