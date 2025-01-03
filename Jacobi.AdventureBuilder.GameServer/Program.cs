using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameActors;
using Jacobi.AdventureBuilder.GameServer;

//
// Game Server
//

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeyedAzureTableClient("game-clusters");
builder.AddKeyedAzureBlobClient("game-grains");
builder.AddApiClient();
builder.AddNotifyPassage();
builder.UseOrleans();

builder.Services.AddGrainServices();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    ;

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();
