using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal static class AdventureMapper
{
    public static AdventureWorldInfo ToWorldInfo(AdventureWorldData worldData)
    {
        var passages = worldData.Passages.Select(passageData => new AdventurePassageInfo
        {
            Id = passageData.Id,
            Name = passageData.Name,
            Description = passageData.Description ?? String.Empty,
            Commands = passageData.Commands.Select(commandData => new AdventureCommandInfo
            {
                Id = $"nav-{commandData.Name}",
                Name = commandData.Name,
                Description = commandData.Description ?? String.Empty,
                Action = commandData.Action,
                Kind = "nav-passage"
            }).ToList(),
            Extras = []
        }).ToList();

        return new AdventureWorldInfo
        {
            Id = worldData.id,
            Name = worldData.Name,
            Passages = passages,
            NonPlayerCharacters = worldData.NonPlayerCharacters.Select(npc => new AdventureNonPlayerCharacterInfo
            {
                Id = npc.Id,
                Name = npc.Name,
                Description = npc.Description,
                LinkedPassages = npc.LinkedPassageIds
                    .Select(id => passages.First(p => p.Id == id))
                    .ToList()
            }).ToList()
        };
    }

    public static AdventureWorldData ToWorldData(AdventureWorldInfo worldInfo)
    {
        return new AdventureWorldData(worldInfo.Id)
        {
            Name = worldInfo.Name,
            Description = "",
            Passages = worldInfo.Passages.Select(passageInfo => new AdventureWorldData.AdventurePassageData
            {
                Id = passageInfo.Id,
                Name = passageInfo.Name,
                Description = passageInfo.Description,
                Commands = passageInfo.Commands.Select(commandInfo => new AdventureWorldData.AdventureCommandData
                {
                    Name = commandInfo.Name,
                    Description = commandInfo.Description,
                    Action = commandInfo.Action
                }).ToList()
            }).ToList(),
            NonPlayerCharacters = worldInfo.NonPlayerCharacters.Select(npc => new AdventureWorldData.AdventureNonPlayerCharacterData
            {
                Id = npc.Id,
                Name = npc.Name,
                Description = npc.Description,
                LinkedPassageIds = npc.LinkedPassages.Select(p => p.Id).ToList()
            }
            ).ToList()
        };
    }
}
