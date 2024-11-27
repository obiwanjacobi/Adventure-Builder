using Jacobi.AdventureBuilder.ApiService.Data;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal interface IAdventureRepository
{
    Task<AdventureWorldData> GetAdventureWorldAsync(string adventureId, CancellationToken ct);
    Task PutAdventureWorldAsync(AdventureWorldData adventureWorld, CancellationToken ct);
}

internal sealed class AdventureRepository : IAdventureRepository
{
    private readonly IDatabase _database;

    public AdventureRepository(IDatabase database)
    {
        _database = database;
        _database.InitializeEntity<AdventureWorldData>();
    }

    public async Task<AdventureWorldData> GetAdventureWorldAsync(string adventureId, CancellationToken ct)
    {
        var world = await _database.LoadAsync<AdventureWorldData>(adventureId, ct);
        return world;
    }

    public async Task PutAdventureWorldAsync(AdventureWorldData adventureWorld, CancellationToken ct)
    {
        var world = await _database.SaveAsync(adventureWorld, ct);
        //return world;
    }
}
