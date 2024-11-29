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

if (builder.Environment.IsDevelopment())
{
    cosmos.WithHttpsEndpoint(8081, 8081, "emulator-port")
        .RunAsEmulator(config => config.WithLifetime(ContainerLifetime.Persistent));

    storage.RunAsEmulator(config => config.WithLifetime(ContainerLifetime.Persistent));
}

var apiService = builder.AddProject<Projects.Jacobi_AdventureBuilder_ApiService>("apiservice")
    .WithReference(cosmos)
    .WaitFor(cosmosDb)
    ;

var gameServer = builder.AddProject<Projects.Jacobi_AdventureBuilder_GameServer>("gameserver")
    .WithReference(apiService).WaitFor(apiService)
    .WithReference(orleans).WithReplicas(1)
    ;

builder.AddProject<Projects.Jacobi_AdventureBuilder_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService).WaitFor(apiService)
    .WithReference(orleans.AsClient()).WithReplicas(1)
    .WaitFor(gameServer)
    ;

using var app = builder.Build();
await app.RunAsync();