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
var gamePubSub = storage.AddTables("game-pubsub");
var gameEvents = storage.AddQueues("game-events");

// orleans actor model
var orleans = builder.AddOrleans("default")
    .WithClustering(gameClusters)
    .WithGrainStorage("Default", grainBlobs)
    .WithGrainStorage("PubSubStore", gamePubSub)
    //.WithStreaming("AzureQueueStorage", gameEvents)
    ;

// Identity Provider
// https://learn.microsoft.com/en-us/dotnet/aspire/authentication/keycloak-integration
// Open resource details in the dashboard for the IdentityProvider and view env vars for admin + pwd.
var identityProvider = builder.AddKeycloak("IdentityProvider", 8080)
    .WithLifetime(ContainerLifetime.Persistent);

if (builder.Environment.IsDevelopment())
{
    cosmos.RunAsEmulator(config => config
        .WithHttpEndpoint(targetPort: 1234, name: "explorer-port", isProxied: false)
        .WithImageRegistry("mcr.microsoft.com")
        .WithImage("cosmosdb/linux/azure-cosmos-emulator")
        .WithImageTag("vnext-preview")
        .WithArgs("--protocol", "https")
        .WithArgs("--explorer-protocol", "http")
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
    .WithReference(gameEvents)
    ;

var webApp = builder.AddProject<Projects.Jacobi_AdventureBuilder_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService).WaitFor(apiService)
    .WithReference(orleans.AsClient()).WithReplicas(1)
    .WaitFor(gameServer)
    .WithReference(identityProvider)
    ;

// notifications back to web app
gameServer.WithReference(webApp);

using var app = builder.Build();
app.Run();
