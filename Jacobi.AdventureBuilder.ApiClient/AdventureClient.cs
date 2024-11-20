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
                    Commands = [
                        new AdventureCommandInfo
                        {
                            Id = "nav-north",
                            Kind = "nav-room",
                            Name = "North",
                            Description = "To the north there is a door leading to another room.",
                            Action = "nav:room:2"
                        }
                    ]
                },
                new AdventureRoomInfo()
                {
                    Id = 2,
                    Name = "Room 2",
                    Description = "You are in Room 2",
                    Commands = [
                        new AdventureCommandInfo
                        {
                            Id = "nav-south",
                            Kind = "nav-room",
                            Name = "South",
                            Description = "To the south there is a door leading to another room.",
                            Action = "nav:room:1"
                        }
                    ]
                }
            ]
        });
    }
}
