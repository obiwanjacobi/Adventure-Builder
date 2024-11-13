namespace Jacobi.AdventureBuilder.ApiClient;

public interface IAdventureClient
{
    AdventureWorldMap GetAdventure(string adventureId);
}

public sealed class AdventureWorldMap
{
    public string Name { get; set; } = String.Empty;
}
