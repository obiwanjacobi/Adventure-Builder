using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.ServiceDefaults;
using Jacobi.AdventureBuilder.Web;
using Microsoft.FluentUI.AspNetCore.Components;

//
// Web
//

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddIdentityAuthentication(
    configuration.GetRequiredValue<string>("WebClientId"),
    configuration.GetRequiredValue<string>("WebClientSecret"));
builder.AddGameClient();

// Add services to the container.
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();
//builder.Services.AddOutputCache();
//builder.Services.AddHttpClient<WeatherApiClient>(client =>
//    {
//        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
//        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
//        client.BaseAddress = new("https://apiservice");
//    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseStaticFiles();
app.MapStaticAssets();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
//app.UseOutputCache();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();

await app.RunAsync();
