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
// parameters are stored in appsettings.json
var keycloakAdmin = builder.AddParameter("KeycloakAdmin");
var keycloakAdminPwd = builder.AddParameter("KeycloakAdminPassword", secret: true);
var identityProvider = builder.AddKeycloak("IdentityProvider", 8080, keycloakAdmin, keycloakAdmin)
    .WithLifetime(ContainerLifetime.Persistent);

if (builder.Environment.IsDevelopment())
{
    cosmos.RunAsEmulator(config => config
        .WithHttpEndpoint(targetPort: 1234, name: "explorer-port", isProxied: false)
        .WithImageRegistry("mcr.microsoft.com")
        .WithImage("cosmosdb/linux/azure-cosmos-emulator")
        .WithImageTag("vnext-preview")
        .WithArgs("--protocol", "https")
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
    ;

using var app = builder.Build();
await app.RunAsync();
