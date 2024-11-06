using Microsoft.Azure.Cosmos;
using Cosmos = Microsoft.Azure.Cosmos;

namespace Jacobi.AdventureBuilder.ApiService.Data;

internal interface IDatabase
{
    Task<T> CreateAsync<T>(T entity, CancellationToken ct) where T : Entity;
}

internal sealed class CosmosDatabase : IDatabase
{
    private readonly Cosmos.CosmosClient _cosmosClient;
    private readonly Cosmos.Database _cosmosDb;

    public const string DatabaseName = "adventurebuilder";

    public CosmosDatabase(Cosmos.CosmosClient client)
    {
        _cosmosClient = client;
        _cosmosDb = _cosmosClient.GetDatabase(DatabaseName);
    }

    public async Task<T> CreateAsync<T>(T entity, CancellationToken ct)
        where T : Entity
    {
        var type = typeof(T).Name;
        var container = await GetContainerAsync(type, ct);
        var response = await container.UpsertItemAsync<T>(entity, new PartitionKey(entity.id), cancellationToken: ct);

        return response.Resource;
    }

    private async Task<Cosmos.Container> GetContainerAsync(string container, CancellationToken ct)
    {
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName, cancellationToken: ct);
        await _cosmosDb.CreateContainerIfNotExistsAsync(container, $"/id", cancellationToken: ct);
        return _cosmosDb.GetContainer(container);
    }
}
