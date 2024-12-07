using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiClient;

public interface IAdventureClient
{
    // full hierarchy
    Task<AdventureWorldInfo> GetAdventureWorldAsync(string worldId, CancellationToken ct = default);
    Task UpsertAdventureWorldAsync(AdventureWorldInfo adventureWorld, CancellationToken ct = default);

    // single objects
    Task<AdventureWorldInfo> GetAdventureWorldSummaryAsync(string worldId, CancellationToken ct = default);
    Task<AdventurePassageInfo> GetAdventurePassageAsync(string worldId, long passageId, CancellationToken ct = default);
    Task<AdventureNonPlayerCharacterInfo> GetAdventureNonPlayerCharacter(string worldId, long npcId, CancellationToken ct = default);
    Task<AdventureAssetInfo> GetAdventureAsset(string worldId, long assetId, CancellationToken ct = default);
}

internal sealed class AdventureClient : IAdventureClient
{
    private static JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient httpClient;

    public AdventureClient(HttpClient client)
        => this.httpClient = client;

    public Task<AdventureWorldInfo> GetAdventureWorldAsync(string worldId, CancellationToken ct = default)
        => GetRequest<AdventureWorldInfo>($"/adventure/worlds/{worldId.ToLowerInvariant()}", ct);

    public async Task UpsertAdventureWorldAsync(AdventureWorldInfo adventureWorld, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(adventureWorld);
        var worldId = adventureWorld.Id;
        var content = new StringContent(json, new MediaTypeHeaderValue(MediaTypeNames.Application.Json));
        var response = await this.httpClient.PutAsync($"/adventure/worlds/{worldId.ToLowerInvariant()}", content, ct);
        response.EnsureSuccessStatusCode();
    }

    public Task<AdventureWorldInfo> GetAdventureWorldSummaryAsync(string worldId, CancellationToken ct = default)
        => GetRequest<AdventureWorldInfo>($"/adventure/worlds/{worldId.ToLowerInvariant()}?summary=true", ct);

    public Task<AdventurePassageInfo> GetAdventurePassageAsync(string worldId, long passageId, CancellationToken ct = default)
        => GetRequest<AdventurePassageInfo>($"/adventure/worlds/{worldId.ToLowerInvariant()}/passages/{passageId}", ct);

    public Task<AdventureNonPlayerCharacterInfo> GetAdventureNonPlayerCharacter(string worldId, long npcId, CancellationToken ct = default)
        => GetRequest<AdventureNonPlayerCharacterInfo>($"/adventure/worlds/{worldId.ToLowerInvariant()}/npcs/{npcId}", ct);

    public Task<AdventureAssetInfo> GetAdventureAsset(string worldId, long assetId, CancellationToken ct = default)
        => GetRequest<AdventureAssetInfo>($"/adventure/worlds/{worldId.ToLowerInvariant()}/assets/{assetId}", ct);

    private async Task<T> GetRequest<T>(string resource, CancellationToken cancellationToken)
    {
        var response = await this.httpClient.GetAsync(resource, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
        var adventurePassage = JsonSerializer.Deserialize<T>(jsonString, JsonOptions);

        return adventurePassage
            ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}.");
    }
}
