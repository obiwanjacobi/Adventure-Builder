using Microsoft.Azure.Cosmos;
using Cosmos = Microsoft.Azure.Cosmos;

namespace Jacobi.AdventureBuilder.ApiService.Data;

internal interface IDatabase
{
    // call once per process
    Task InitializeDatabase();
    // call once per entity type/class
    Task InitializeEntity<T>() where T : Entity, ILogicalPartition;

    Task<T> SaveAsync<T>(T entity, CancellationToken ct) where T : Entity, ILogicalPartition;
    Task<T> LoadAsync<T>(string id, CancellationToken ct) where T : Entity, ILogicalPartition;
}

internal sealed class CosmosDatabase : IDatabase
{
    private readonly Cosmos.CosmosClient _cosmosClient;
    private readonly Cosmos.Database _cosmosDb;

    public const string DatabaseName = "adventuredata";

    public CosmosDatabase(Cosmos.CosmosClient client)
    {
        _cosmosClient = client;
        _cosmosDb = _cosmosClient.GetDatabase(DatabaseName);
    }

    public Task InitializeDatabase()
        => _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);

    public Task InitializeEntity<T>()
        where T : Entity, ILogicalPartition
        => _cosmosDb.CreateContainerIfNotExistsAsync(T.ContainerName, $"/{T.PartitionPath}");

    public async Task<T> SaveAsync<T>(T entity, CancellationToken ct)
        where T : Entity, ILogicalPartition
    {
        var container = await GetContainerAsync(T.ContainerName, T.PartitionPath, ct);
        var response = await container.UpsertItemAsync<T>(entity, new PartitionKey(entity.PartitionKey), cancellationToken: ct);

        return response.Resource;
    }

    public async Task<T> LoadAsync<T>(string id, CancellationToken ct)
        where T : Entity, ILogicalPartition
    {
        var container = await GetContainerAsync(T.ContainerName, T.PartitionPath, ct);
        var response = await container.ReadItemAsync<T>(id, new PartitionKey(id));
        return response.Resource;
    }

    private async Task<Cosmos.Container> GetContainerAsync(string container, string partitionPath, CancellationToken ct)
    {
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName, cancellationToken: ct);
        await _cosmosDb.CreateContainerIfNotExistsAsync(container, $"/{partitionPath}", cancellationToken: ct);
        return _cosmosDb.GetContainer(container);
    }
}
