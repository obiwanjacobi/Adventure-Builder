using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal static class AdventureMapper
{
    public static AdventureWorldInfo ToWorldInfo(AdventureWorldData worldData)
    {
        return new AdventureWorldInfo
        {
            Id = worldData.id,
            Name = worldData.Name,
            Rooms = worldData.Rooms.Select(roomData => new AdventureRoomInfo
            {
                Id = roomData.Id,
                Name = roomData.Name,
                Description = roomData.Description ?? String.Empty,
                Commands = roomData.Commands.Select(commandData => new AdventureCommandInfo
                {
                    Id = $"nav-{commandData.Name}",
                    Name = commandData.Name,
                    Description = commandData.Description ?? String.Empty,
                    Action = commandData.Action,
                    Kind = "nav-room"
                }).ToList()
            }).ToList()
        };
    }

    public static AdventureWorldData ToWorldData(AdventureWorldInfo worldInfo)
    {
        return new AdventureWorldData(worldInfo.Id)
        {
            Name = worldInfo.Name,
            Description = "",
            Rooms = worldInfo.Rooms.Select(roomInfo => new AdventureWorldData.AdventureRoomData
            {
                Id = roomInfo.Id,
                Name = roomInfo.Name,
                Description = roomInfo.Description,
                Commands = roomInfo.Commands.Select(commandInfo => new AdventureWorldData.AdventureCommandData
                {
                    Name = commandInfo.Name,
                    Description = commandInfo.Description,
                    Action = commandInfo.Action
                }).ToList()
            }).ToList()
        };
    }
}
