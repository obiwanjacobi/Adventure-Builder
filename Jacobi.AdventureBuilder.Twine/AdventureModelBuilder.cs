using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.Twine;

internal sealed class AdventureModelBuilder
{
    private readonly List<AdventurePassageInfo> _passages = [];
    private readonly List<AdventureNonPlayerCharacterInfo> _npcs = [];
    private readonly List<AdventureAssetInfo> _assets = [];

    public AdventurePassageInfo AddPassage(long id, string name, string description, IReadOnlyList<AdventureLinkInfo> commands)
    {
        var passage = new AdventurePassageInfo()
        {
            Id = id,
            Name = name,
            Description = description,
            LinkedPassages = commands,
        };

        _passages.Add(passage);
        return passage;
    }

    public static AdventureLinkInfo CreateNavigateLink(string passageId, string name, string description)
    {
        return new AdventureLinkInfo
        {
            PassageId = Int64.Parse(passageId),
            Name = name,
            Description = description,
        };
    }

    public AdventureWorldInfo Build(string id, string name)
    {
        var world = new AdventureWorldInfo
        {
            Id = id,
            Name = name,
            // make copies of the lists!
            Passages = _passages.ToList(),
            NonPlayerCharacters = _npcs.ToList(),
            Assets = _assets.ToList(),
        };

        _passages.Clear();
        _npcs.Clear();
        _assets.Clear();

        return world;
    }

    public AdventureNonPlayerCharacterInfo AddNonPlayerCharacter(long id, string name, string description, IReadOnlyList<long> linkedPassageIds, IReadOnlyList<AdventurePropertyInfo> properties)
    {
        var npc = new AdventureNonPlayerCharacterInfo
        {
            Id = id,
            Name = name,
            Description = description,
            LinkedPassageIds = linkedPassageIds,
            Properties = properties
        };

        _npcs.Add(npc);
        return npc;
    }

    public AdventureAssetInfo AddAsset(long id, string name, string description, IReadOnlyList<long> linkedPassageIds, IReadOnlyList<AdventurePropertyInfo> properties)
    {
        var asset = new AdventureAssetInfo
        {
            Id = id,
            Name = name,
            Description = description,
            LinkedPassageIds = linkedPassageIds,
            Properties = properties
        };

        _assets.Add(asset);
        return asset;
    }
}
