using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiClient;

public interface IAdventureClient
{
    Task<AdventureWorldInfo> GetAdventureWorldAsync(string worldId, CancellationToken ct = default);
    Task UpsertAdventureWorldAsync(AdventureWorldInfo adventureWorld, CancellationToken ct = default);
}

internal sealed class AdventureClient : IAdventureClient
{
    private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient httpClient;

    public AdventureClient(HttpClient client)
        => this.httpClient = client;

    public async Task<AdventureWorldInfo> GetAdventureWorldAsync(string worldId, CancellationToken ct = default)
    {
        var response = await this.httpClient.GetAsync($"/adventure/worlds/{worldId.ToLowerInvariant()}", ct);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync(ct);
        var adventureWorld = JsonSerializer.Deserialize<AdventureWorldInfo>(jsonString, JsonOptions);

        return adventureWorld
            ?? throw new JsonException("Failed to deserialize AdventureWorldInfo.");
    }

    public async Task UpsertAdventureWorldAsync(AdventureWorldInfo adventureWorld, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(adventureWorld);
        var worldId = adventureWorld.Id;
        var content = new StringContent(json, new MediaTypeHeaderValue(MediaTypeNames.Application.Json));
        var response = await this.httpClient.PutAsync($"/adventure/worlds/{worldId.ToLowerInvariant()}", content, ct);
        response.EnsureSuccessStatusCode();
    }
}
