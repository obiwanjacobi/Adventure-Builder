using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiService.Data;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal interface IAdventureRepository
{
    Task<AdventureWorldInfo> GetAdventureWorldAsync(string adventureId, CancellationToken ct);
}

internal sealed class AdventureRepository : IAdventureRepository
{
    private readonly IDatabase _database;

    public AdventureRepository(IDatabase database)
    {
        _database = database;
    }

    public Task<AdventureWorldInfo> GetAdventureWorldAsync(string adventureId, CancellationToken ct)
    {
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
