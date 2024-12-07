using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal static class AdventureMapper
{
    public static AdventureWorldInfo ToWorldInfoSummary(AdventureWorldData worldData)
    {
        return new AdventureWorldInfo
        {
            Id = worldData.id,
            Name = worldData.Name,
            Passages = worldData.Passages.Select(passageData => new AdventurePassageInfo
            {
                Id = passageData.Id,
                Name = passageData.Name,
                Description = String.Empty,
                Commands = [],
                Extras = []
            }).ToList(),
            NonPlayerCharacters = worldData.NonPlayerCharacters
                .Select(ToNonPlayerCharacterInfo)
                .ToList(),
            Assets = worldData.Assets
                .Select(ToAssetInfo)
                .ToList(),
        };
    }

    public static AdventureWorldInfo ToWorldInfo(AdventureWorldData worldData)
    {
        var passages = worldData.Passages.Select(ToPassageInfo).ToList();

        return new AdventureWorldInfo
        {
            Id = worldData.id,
            Name = worldData.Name,
            Passages = passages,
            NonPlayerCharacters = worldData.NonPlayerCharacters
                .Select(ToNonPlayerCharacterInfo)
                .ToList(),
            Assets = worldData.Assets
                .Select(ToAssetInfo)
                .ToList(),
        };
    }

    public static AdventurePassageInfo ToPassageInfo(AdventureWorldData worldData, long passageId)
    {
        var passageData = worldData.Passages.First(p => p.Id == passageId);
        return ToPassageInfo(passageData);
    }

    public static AdventureNonPlayerCharacterInfo ToNonPlayerCharacterInfo(AdventureWorldData worldData, long npcId)
    {
        var npcData = worldData.NonPlayerCharacters.First(npc => npc.Id == npcId);
        return ToNonPlayerCharacterInfo(npcData);
    }

    public static AdventureAssetInfo ToAssetInfo(AdventureWorldData worldData, long assetId)
    {
        var assetData = worldData.Assets.First(asset => asset.Id == assetId);
        return ToAssetInfo(assetData);
    }

    private static AdventurePassageInfo ToPassageInfo(AdventureWorldData.AdventurePassageData passageData)
    {
        return new AdventurePassageInfo
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
        };
    }

    private static AdventureNonPlayerCharacterInfo ToNonPlayerCharacterInfo(AdventureWorldData.AdventureNonPlayerCharacterData npcData)
    {
        return new AdventureNonPlayerCharacterInfo
        {
            Id = npcData.Id,
            Name = npcData.Name,
            Description = npcData.Description,
            LinkedPassageIds = npcData.LinkedPassageIds
        };
    }

    private static AdventureAssetInfo ToAssetInfo(AdventureWorldData.AdventureAssetData assetData)
    {
        return new AdventureAssetInfo
        {
            Id = assetData.Id,
            Name = assetData.Name,
            Description = assetData.Description,
            LinkedPassageIds = assetData.LinkedPassageIds,
        };
    }

    //-------------------------------------------------------------------------

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
                LinkedPassageIds = [.. npc.LinkedPassageIds]
            }
            ).ToList(),
            Assets = worldInfo.Assets.Select(asset => new AdventureWorldData.AdventureAssetData
            {
                Id = asset.Id,
                Name = asset.Name,
                Description = asset.Description,
                LinkedPassageIds = [.. asset.LinkedPassageIds]
            }).ToList()
        };
    }
}
