namespace Jacobi.AdventureBuilder.ApiClient;

internal class AdventureClient : IAdventureClient
{
    public AdventureWorldMap GetAdventure(string adventureId)
    {
        return new AdventureWorldMap()
        {
            Name = adventureId
        };
    }
}
