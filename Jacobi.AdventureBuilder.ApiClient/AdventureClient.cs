using System.Text.Json;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiClient;

public interface IAdventureClient
{
    Task<AdventureWorldInfo> GetAdventureWorldAsync(string worldId, CancellationToken ct);
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

    public async Task<AdventureWorldInfo> GetAdventureWorldAsync(string worldId, CancellationToken ct)
    {
        var response = await this.httpClient.GetAsync($"/adventure/worlds/{worldId}", ct);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync(ct);
        var adventureWorld = JsonSerializer.Deserialize<AdventureWorldInfo>(jsonString, JsonOptions);

        return adventureWorld ?? throw new JsonException("Failed to deserialize AdventureWorldInfo.");
    }
}
