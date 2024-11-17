using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiClient;

public interface IAdventureClient
{
    Task<AdventureWorldInfo> GetAdventureWorldAsync(string adventureId);
}
