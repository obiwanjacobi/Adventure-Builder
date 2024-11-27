using FastEndpoints;
using Jacobi.AdventureBuilder.ApiService.Account;
using Jacobi.AdventureBuilder.ApiService.Adventure;
using Jacobi.AdventureBuilder.ApiService.Data;

//
// API Service
//

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cmos-adventurebuilder");

// Add services to the container.
builder.Services
    .AddProblemDetails()
    .AddFastEndpoints()
    // application specific
    .AddDataServices()
    .AddAccountServices()
    .AddAdventureServices()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.UseFastEndpoints();

await app.RunAsync();
