using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Jacobi.AdventureBuilder.ServiceDefaults;

public static class AuthenticationExtensions
{
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
            realm: "AdevntureBuilder",
            options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                //options.Scope.Add("game-server");
                //options.Scope.Add("api-service");
                //options.Scope.Add("web-frontend");
                options.SaveTokens = true;
            });
    }
}
