using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ServiceDiscovery;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Jacobi.AdventureBuilder.ServiceDefaults;

public static class AuthenticationExtensions
{
    const string OpenIdConnectBackchannel = "OpenIdConnectBackchannel";

    public static void AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        builder.Services.AddHttpClient(OpenIdConnectBackchannel, o => o.BaseAddress = new("http://IdentityProvider"));

        var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

        // Add Authentication services
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
            .AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
            .AddOpenIdConnect()
            .ConfigureWebAppOpenIdConnect();

        // Blazor auth services
        services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        services.AddCascadingAuthenticationState();

        services.AddSingleton<ILogonService, LogonService>();
    }

    private static void ConfigureWebAppOpenIdConnect(this AuthenticationBuilder authentication)
    {
        // Named options
        authentication.Services.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<IConfiguration, IHttpClientFactory, IHostEnvironment>(configure);

        // Unnamed options
        authentication.Services.AddOptions<OpenIdConnectOptions>()
            .Configure<IConfiguration, IHttpClientFactory, IHostEnvironment>(configure);

        static void configure(OpenIdConnectOptions options, IConfiguration configuration, IHttpClientFactory httpClientFactory, IHostEnvironment hostEnvironment)
        {
            var clientSecret = configuration
                .GetRequiredSection("Identity")
                .GetValue<string>("ClientSecret");
            var backchannelHttpClient = httpClientFactory.CreateClient(OpenIdConnectBackchannel);

            options.Backchannel = backchannelHttpClient;
            options.Authority = backchannelHttpClient.GetIdentityProviderAuthorityUri(configuration).ToString();
            options.ClientId = "webapp";
            options.ClientSecret = clientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true; // Preserve the access token so it can be used to call backend APIs
            options.RequireHttpsMetadata = !hostEnvironment.IsDevelopment();
            options.MapInboundClaims = false; // Prevent from mapping "sub" claim to nameidentifier.
        }
    }

    public static async Task<string?> GetBuyerIdAsync(this AuthenticationStateProvider authenticationStateProvider)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.GetUserId();
    }

    public static async Task<string?> GetUserNameAsync(this AuthenticationStateProvider authenticationStateProvider)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.GetUserName();
    }

    public static Uri GetIdentityProviderAuthorityUri(this HttpClient httpClient, IConfiguration configuration)
    {
        var idpBaseUri = httpClient.BaseAddress
            ?? throw new InvalidOperationException($"HttpClient instance does not have a BaseAddress configured.");
        var identityUri = GetIdentityProviderRealmUri(idpBaseUri, configuration);

        return identityUri;
    }

    public static Uri ResolveIdentityProviderAuthorityUri(this ServiceEndpointResolver resolver, IConfiguration configuration, string serviceName = "http://idp")
    {
        // Sync over async :(
        var idpBaseUrl = resolver.ResolveEndpointUrlAsync(serviceName).AsTask().GetAwaiter().GetResult()
            ?? throw new InvalidOperationException($"Could not resolve IdentityProvider address using service name '{serviceName}'.");
        var identityUri = GetIdentityProviderRealmUri(new Uri(idpBaseUrl), configuration);

        return identityUri;
    }

    private static Uri GetIdentityProviderRealmUri(Uri idpBaseUri, IConfiguration configuration)
    {
        var identitySection = configuration.GetSection("Identity");
        var realm = identitySection["Realm"] ?? "AdventureBuilder";

        return new Uri(idpBaseUri, $"realms/{realm}/");
    }

    //public const string JwtBearerBackchannel = "JwtBearerBackchannel";

    //public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    //{
    //    var services = builder.Services;

    //    // {
    //    //   "Identity": {
    //    //     "Audience": "basket"
    //    //    }
    //    // }

    //    builder.Services.AddHttpClient(JwtBearerBackchannel, o => o.BaseAddress = new("http://idp"));

    //    services.AddAuthentication()
    //        .AddJwtBearer()
    //        .ConfigureDefaultJwtBearer();

    //    services.AddAuthorization();

    //    return services;
    //}

    //private static void ConfigureDefaultJwtBearer(this AuthenticationBuilder authentication)
    //{
    //    // Named options
    //    authentication.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    //        .Configure<IConfiguration, IHttpClientFactory>(configure);

    //    // Unnamed options
    //    authentication.Services.AddOptions<JwtBearerOptions>()
    //        .Configure<IConfiguration, IHttpClientFactory>(configure);

    //    static void configure(JwtBearerOptions options, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    //    {
    //        var identitySection = configuration.GetSection("Identity");
    //        var audience = identitySection.GetRequiredValue("Audience");
    //        var backchannelHttpClient = httpClientFactory.CreateClient(JwtBearerBackchannel);

    //        options.Backchannel = backchannelHttpClient;
    //        options.Authority = backchannelHttpClient.GetIdpAuthorityUri(configuration).ToString();
    //        options.RequireHttpsMetadata = false;
    //        options.Audience = audience;
    //        options.TokenValidationParameters.ValidateAudience = false;
    //        // Prevent from mapping "sub" claim to nameidentifier.
    //        options.MapInboundClaims = false;
    //    }
    //}

    public static async ValueTask<string?> ResolveEndpointUrlAsync(this ServiceEndpointResolver resolver, string serviceName, CancellationToken cancellationToken = default)
    {
        var scheme = ExtractScheme(serviceName);
        var endpoints = await resolver.GetEndpointsAsync(serviceName, cancellationToken);
        if (endpoints.Endpoints.Count > 0)
        {
            var address = endpoints.Endpoints[0].ToString();
            return $"{scheme}://{address}";
        }
        return null;
    }

    private static string? ExtractScheme(string serviceName)
    {
        if (Uri.TryCreate(serviceName, UriKind.Absolute, out var uri))
        {
            return uri.Scheme;
        }
        return null;
    }
}

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirst("sub")?.Value;

    public static string? GetUserName(this ClaimsPrincipal principal) =>
        principal.FindFirst(x => x.Type == "name")?.Value;
}

public interface ILogonService
{
    Task LogoutAsync(HttpContext httpContext);
}

internal sealed class LogonService : ILogonService
{
    public string LoginUrl()
        => "./account/login";

    public async Task LoginAsync(HttpContext httpContext, string user, string password)
    {
        var claim = new GenericIdentity(username, password);

        await httpContext.SignInAsync(principal);
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    }
}
