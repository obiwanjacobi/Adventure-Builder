using Microsoft.Extensions.Hosting;

//
// AppHost
//

var builder = DistributedApplication.CreateBuilder(args);

// metadata store
var cosmosDb = builder.AddAzureCosmosDB("adventurebuilder");
cosmosDb.AddDatabase("adventurebuilder");

// runtime store
var storage = builder.AddAzureStorage("stadventurebuilder");
var gameClusters = storage.AddTables("game-clusters");
var grainBlobs = storage.AddBlobs("game-grains");

// orleans actor model
var orleans = builder.AddOrleans("default")
    .WithClustering(gameClusters)
    .WithGrainStorage("Default", grainBlobs)
    ;

if (builder.Environment.IsDevelopment())
{
    cosmosDb.RunAsEmulator();
    storage.RunAsEmulator();
}

var apiService = builder.AddProject<Projects.Jacobi_AdventureBuilder_ApiService>("apiservice")
    .WithReference(cosmosDb)
    .WaitFor(cosmosDb)
    ;

var gameServer = builder.AddProject<Projects.Jacobi_AdventureBuilder_GameServer>("gameserver")
    .WithReference(orleans)
    .WithReplicas(1)
    ;

builder.AddProject<Projects.Jacobi_AdventureBuilder_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(orleans.AsClient())
    .WaitFor(gameServer)
    .WithReplicas(1)
    ;


using var app = builder.Build();
await app.RunAsync();
