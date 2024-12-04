using Jacobi.AdventureBuilder.ApiService.Data;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal interface IAdventureRepository
{
    Task<AdventureWorldData> GetAdventureWorldAsync(string adventureId, CancellationToken ct);
    Task PutAdventureWorldAsync(AdventureWorldData adventureWorld, CancellationToken ct);
}

internal sealed class AdventureRepository : IAdventureRepository
{
    private readonly HybridCache _cache;
    private readonly IDatabase _database;

    public AdventureRepository(IDatabase database, HybridCache cache)
    {
        _cache = cache;
        _database = database;
        _database.InitializeEntity<AdventureWorldData>();
    }

    public async Task<AdventureWorldData> GetAdventureWorldAsync(string adventureId, CancellationToken ct)
    {
        var world = await _cache.GetOrCreateAsync($"World;{adventureId}", async (ct) =>
            await _database.LoadAsync<AdventureWorldData>(adventureId, ct));

        return world;
    }

    public async Task PutAdventureWorldAsync(AdventureWorldData adventureWorld, CancellationToken ct)
    {
        var world = await _database.SaveAsync(adventureWorld, ct);
        await _cache.SetAsync($"World;{world.id}", world);
    }
}
