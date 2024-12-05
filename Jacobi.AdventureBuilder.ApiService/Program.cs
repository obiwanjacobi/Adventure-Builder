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
    .AddOpenApi()
    .AddEndpointsApiExplorer()
    // application specific
    .AddDataServices()
    .AddAccountServices()
    .AddAdventureServices()
    ;

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();
app.UseFastEndpoints();

await app.RunAsync();
