using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Jacobi.AdventureBuilder.ServiceDefaults;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(this IHostApplicationBuilder builder, string audience)
    {
        builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(
                serviceName: "IdentityProvider",
                realm: "AdventureBuilder",
                options =>
                {
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.Audience = audience;
                });

        builder.Services.AddAuthorizationBuilder();
    }

    // https://learn.microsoft.com/en-us/dotnet/aspire/authentication/keycloak-integration
    public static void AddIdentityAuthentication(this IHostApplicationBuilder builder, string clientId, string clientSecret /*, params string[] scopes*/)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddKeycloakOpenIdConnect(
            "IdentityProvider",
            realm: "AdventureBuilder",
            OpenIdConnectDefaults.AuthenticationScheme,
            options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                //options.Scope.Add("AdventureBuilder:all");
                //options.Scope.Add("game-server");
                //options.Scope.Add("api-service");
                //options.Scope.Add("web-frontend");
                options.SaveTokens = true;
                options.CallbackPath = "/account/login";
            });
    }

    public static IEndpointConventionBuilder MapLoginAndLogout(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        group.MapGet(pattern: "/login", OnLogin).AllowAnonymous();
        group.MapPost(pattern: "/logout", OnLogout);

        return group;
    }

    static ChallengeHttpResult OnLogin() =>
        TypedResults.Challenge(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        });

    static SignOutHttpResult OnLogout() =>
        TypedResults.SignOut(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        },
        [
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        ]);
}

public sealed class BearerAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException(
                "No HttpContext available from the IHttpContextAccessor.");

        var accessToken = await httpContext.GetTokenAsync("access_token");

        if (!String.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
