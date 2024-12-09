using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Jacobi.AdventureBuilder.ServiceDefaults;

public static class AuthenticationExtensions
{
    public static void AddIdentityAuthentication(this IHostApplicationBuilder builder)
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
                options.ClientId = "web-frontend";
                options.ResponseType = OpenIdConnectResponseType.Code;
                //options.Scope.Add("game-server");
                //options.Scope.Add("api-service");
                options.SaveTokens = true;
            });
    }
}
