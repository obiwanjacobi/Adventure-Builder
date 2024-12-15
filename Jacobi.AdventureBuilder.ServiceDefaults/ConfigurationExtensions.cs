using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace Jacobi.AdventureBuilder.ServiceDefaults;

public static class ConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        => configuration.GetValue<T>(key)
            ?? throw new InvalidConfigurationException($"Configuration setting '{key}' was not found.");
}
