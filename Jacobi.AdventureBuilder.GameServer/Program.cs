using Azure.Storage.Queues;
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
builder.AddKeyedAzureTableClient("game-pubsub");
builder.AddKeyedAzureQueueClient("game-events");
builder.AddApiClient();
builder.AddNotifyPassage();
builder.UseOrleans(siloBuilder =>
{
    siloBuilder.AddAzureQueueStreams("AzureQueueProvider", configurator =>
    {
        configurator.ConfigureAzureQueue(options =>
        {
            options.Configure<IServiceProvider>((queueOptions, sp) =>
            {
                queueOptions.QueueServiceClient = sp.GetKeyedService<QueueServiceClient>("game-events");
                queueOptions.QueueNames = ["adventure-events-azurequeueprovider"];
            });
        });
        configurator.ConfigurePullingAgent(ob => ob.Configure(options =>
        {
            options.GetQueueMsgsTimerPeriod = TimeSpan.FromMilliseconds(1000);
            //options.StreamInactivityPeriod = TimeSpan.FromMilliseconds(1000);
        }));
    });

    siloBuilder.ConfigureLogging(logging =>
    {
        logging.AddConsole();
#if DEBUG
        logging.AddDebug();
#endif
    });
});

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
