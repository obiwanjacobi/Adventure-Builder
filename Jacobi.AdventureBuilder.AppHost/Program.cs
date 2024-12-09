using Jacobi.AdventureBuilder.AppHost;
using Microsoft.Extensions.Hosting;

//
// AppHost
//

var builder = DistributedApplication.CreateBuilder(args);

// metadata store
var cosmos = builder.AddAzureCosmosDB("cmos-adventurebuilder");
var cosmosDb = cosmos.AddDatabase("adventuredata");

// runtime store
var storage = builder.AddAzureStorage("st-adventurebuilder");
var gameClusters = storage.AddTables("game-clusters");
var grainBlobs = storage.AddBlobs("game-grains");

// orleans actor model
var orleans = builder.AddOrleans("default")
    .WithClustering(gameClusters)
    .WithGrainStorage("Default", grainBlobs)
    ;

// Identity Provider
// https://learn.microsoft.com/en-us/dotnet/aspire/authentication/keycloak-integration
// https://github.com/dotnet-presentations/eshop-app-workshop/tree/main/labs/3-Add-Identity
//var identityProvider = builder.AddKeycloakContainer("IdentityProvider" /*, tag: "23.0"*/)
//    .WithLifetime(ContainerLifetime.Persistent)
//    .ImportRealms("./Keycloak/data");

// parameters in appsettings.json
var keycloakAdmin = builder.AddParameter("KeycloakAdmin");
var keycloakAdminPwd = builder.AddParameter("KeycloakAdminPassword", secret: true);
var identityProvider = builder.AddKeycloak("IdentityProvider", 8080, keycloakAdmin, keycloakAdmin)
    .WithLifetime(ContainerLifetime.Persistent);

if (builder.Environment.IsDevelopment())
{
    cosmos
        .RunAsEmulator(config => config
            //.WithHttpEndpoint(targetPort: 1234, name: "explorer-port")
            //.WithImageRegistry("mcr.microsoft.com")
            //.WithImage("cosmosdb/linux/azure-cosmos-emulator")
            //.WithImageTag("vnext-preview")
            .WithLifetime(ContainerLifetime.Persistent)
        );

    // Seems to cause problems when debugging (and stopping half way).
    //storage.RunAsEmulator(config => config.WithLifetime(ContainerLifetime.Persistent));
    storage.RunAsEmulator();
}

var apiService = builder.AddProject<Projects.Jacobi_AdventureBuilder_ApiService>("apiservice")
    .WithReference(cosmos)
    .WaitFor(cosmosDb)
    ;

var gameServer = builder.AddProject<Projects.Jacobi_AdventureBuilder_GameServer>("gameserver")
    .WithReference(apiService).WaitFor(apiService)
    .WithReference(orleans).WithReplicas(1)
    ;

var webApp = builder.AddProject<Projects.Jacobi_AdventureBuilder_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService).WaitFor(apiService)
    .WithReference(orleans.AsClient()).WithReplicas(1)
    .WaitFor(gameServer)
    .WithReference(identityProvider)
    //.WithReference(identityProvider, env: "Identity__ClientSecret")
    ;

//var webAppHttp = webApp.GetEndpoint("http");
//identityProvider.WithEnvironment("WEBAPP_HTTP_CONTAINERHOST", webAppHttp);
//identityProvider.WithEnvironment("WEBAPP_HTTP", () => $"{webAppHttp.Scheme}://{webAppHttp.Host}:{webAppHttp.Port}");
//var webAppHttps = webApp.GetEndpoint("https");
//identityProvider.WithEnvironment("WEBAPP_HTTPS_CONTAINERHOST", webAppHttps);
//identityProvider.WithEnvironment("WEBAPP_HTTPS", () => $"{webAppHttps.Scheme}://{webAppHttps.Host}:{webAppHttps.Port}");

using var app = builder.Build();
await app.RunAsync();
