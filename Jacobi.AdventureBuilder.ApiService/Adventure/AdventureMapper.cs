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
                LinkedPassages = [],
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
            LinkedPassages = passageData.LinkedPassages.Select(linkData => new AdventureLinkInfo
            {
                PassageId = linkData.PassageId,
                Name = linkData.Name,
                Description = linkData.Description ?? String.Empty,
            }).ToList(),
        };
    }

    private static AdventureNonPlayerCharacterInfo ToNonPlayerCharacterInfo(AdventureWorldData.AdventureNonPlayerCharacterData npcData)
    {
        return new AdventureNonPlayerCharacterInfo
        {
            Id = npcData.Id,
            Name = npcData.Name,
            Description = npcData.Description,
            LinkedPassageIds = npcData.LinkedPassageIds,
            Properties = ToPropertyInfos(npcData.Properties),
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
            Properties = ToPropertyInfos(assetData.Properties),
        };
    }

    private static IReadOnlyList<AdventurePropertyInfo> ToPropertyInfos(IEnumerable<AdventureWorldData.AdventurePropertyData> properties)
    {
        return properties.Select(p => new AdventurePropertyInfo
        {
            Name = p.Name,
            Value = p.Value
        }).ToList();
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
                LinkedPassages = passageInfo.LinkedPassages.Select(linkInfo => new AdventureWorldData.AdventureLinkData
                {
                    PassageId = linkInfo.PassageId,
                    Name = linkInfo.Name,
                    Description = linkInfo.Description,
                }).ToList(),
            }).ToList(),
            NonPlayerCharacters = worldInfo.NonPlayerCharacters.Select(npc => new AdventureWorldData.AdventureNonPlayerCharacterData
            {
                Id = npc.Id,
                Name = npc.Name,
                Description = npc.Description,
                LinkedPassageIds = [.. npc.LinkedPassageIds],
                Properties = ToPropertyData(npc.Properties),
            }
            ).ToList(),
            Assets = worldInfo.Assets.Select(asset => new AdventureWorldData.AdventureAssetData
            {
                Id = asset.Id,
                Name = asset.Name,
                Description = asset.Description,
                LinkedPassageIds = [.. asset.LinkedPassageIds],
                Properties = ToPropertyData(asset.Properties),
            }).ToList(),
        };
    }

    private static List<AdventureWorldData.AdventurePropertyData> ToPropertyData(IEnumerable<AdventurePropertyInfo> properties)
        => properties
            .Select(p => new AdventureWorldData.AdventurePropertyData
            { Name = p.Name, Value = p.Value })
            .ToList();
}
