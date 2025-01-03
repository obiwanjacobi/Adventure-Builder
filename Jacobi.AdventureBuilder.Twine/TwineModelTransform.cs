using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.Twine;

internal class TwineModelTransform
{
    private readonly AdventureModelBuilder _builder = new();
    private TwineModel? _twineModel;

    public AdventureWorldInfo Transform(TwineModel twineModel)
    {
        _twineModel = twineModel;
        OnPassages(twineModel.Passages);
        _twineModel = null;
        return _builder.Build(twineModel.Uuid.ToString(), twineModel.Name);
    }

    private void OnPassages(List<Passage>? passages)
    {
        if (passages is null) return;

        var npcs = new List<Passage>();
        var assets = new List<Passage>();
        var passageInfos = new List<AdventurePassageInfo>();

        foreach (var passage in passages)
        {
            var properties = CreateProperties(passage.Tags);

            if (IsNPC(properties))
            {
                npcs.Add(passage);
            }
            else if (IsAsset(properties))
            {
                assets.Add(passage);
            }
            else
            {
                var links = CreateLinks(passage);
                var passageInfo = _builder.AddPassage(
                    Int64.Parse(passage.Id), passage.Name, passage.CleanText, links);
                passageInfos.Add(passageInfo);
            }
        }

        // now all the passages are known, process npcs and assets
        foreach (var npc in npcs)
        {
            var properties = CreateProperties(npc.Tags);
            var linkedPassageIds = npc.Links.Select(l => passageInfos.First(p => p.Name == l.PassageName).Id);
            _builder.AddNonPlayerCharacter(
                Int64.Parse(npc.Id), npc.Name, npc.CleanText, linkedPassageIds.ToList(), properties);
        }
        foreach (var asset in assets)
        {
            var properties = CreateProperties(asset.Tags);
            var linkedPassageIds = asset.Links.Select(l => passageInfos.First(p => p.Name == l.PassageName).Id);
            _builder.AddAsset(
                Int64.Parse(asset.Id), asset.Name, asset.CleanText, linkedPassageIds.ToList(), properties);
        }

        static bool IsNPC(List<AdventurePropertyInfo> properties)
            => IsOfType(properties, "npc");
        static bool IsAsset(List<AdventurePropertyInfo> properties)
            => IsOfType(properties, "asset");
        static bool IsOfType(List<AdventurePropertyInfo> properties, string type)
            => properties.Find(prop => prop.Name == "type" && prop.Value == type) is not null;
    }

    private static List<AdventurePropertyInfo> CreateProperties(string tagString)
    {
        var tags = tagString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return tags.Select(tag =>
        {
            var nameValue = tag.Split(':');
            return new AdventurePropertyInfo
            {
                Name = nameValue[0],
                Value = nameValue[1]
            };
        }).ToList();
    }

    private List<AdventureLinkInfo> CreateLinks(Passage passage)
    {
        if (passage.Links is null) return [];
        var links = passage.Links;
        var commands = new List<AdventureLinkInfo>(links.Count);

        foreach (var link in links)
        {
            var passageId = LookupPassageId(link.PassageName);
            var description = LookupLinkDescription(passage.Hooks, link.PassageName)
                ?? link.LinkText;
            var command = AdventureModelBuilder.CreateNavigateLink(passageId, link.LinkText, description);
            commands.Add(command);
        }

        return commands;
    }

    private static string? LookupLinkDescription(List<Hook> hooks, string passageName)
        => hooks.FirstOrDefault(h => h.HookName == passageName)?.HookText;

    private string LookupPassageId(string passageName)
        => _twineModel!.Passages.First(p => p.Name == passageName).Id;
}
