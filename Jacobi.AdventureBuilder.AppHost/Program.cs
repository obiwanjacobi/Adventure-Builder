using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cosmosDb = builder.AddAzureCosmosDB("adventurebuilder");
cosmosDb.AddDatabase("adventurebuilder");
if (builder.Environment.IsDevelopment())
    cosmosDb.RunAsEmulator();


var apiService = builder.AddProject<Projects.Jacobi_AdventureBuilder_ApiService>("apiservice")
    .WithReference(cosmosDb)
    ;

builder.AddProject<Projects.Jacobi_AdventureBuilder_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.AddProject<Projects.Jacobi_AdventureBuilder_GameServer>("gameserver");

builder.Build().Run();
