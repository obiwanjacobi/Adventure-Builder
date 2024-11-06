using FastEndpoints;
using Jacobi.AdventureBuilder.ApiService.Account;
using Jacobi.AdventureBuilder.ApiService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("adventurebuilder");

// Add services to the container.
builder.Services
    .AddProblemDetails()
    .AddFastEndpoints()
    // application specific
    .AddDataServices()
    .AddAccountServices()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.UseFastEndpoints();
app.Run();
