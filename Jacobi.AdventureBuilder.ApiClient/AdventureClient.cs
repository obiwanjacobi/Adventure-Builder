using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiClient;

internal sealed class AdventureClient : IAdventureClient
{
    public Task<AdventureWorldInfo> GetAdventureWorldAsync(string adventureId)
    {
        // TODO: get from metadata database

        return Task.FromResult(new AdventureWorldInfo()
        {
            Id = adventureId,
            Name = $"Adventure {adventureId}",
            Rooms = [
                new AdventureRoomInfo()
                {
                    Id = 1,
                    Name = "Room 1",
                    Description = "You are in Room 1",
                },
                new AdventureRoomInfo()
                {
                    Id = 2,
                    Name = "Room 2",
                    Description = "You are in Room 2",
                }
            ]
        });
    }
}
