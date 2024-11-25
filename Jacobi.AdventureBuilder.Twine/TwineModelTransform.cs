﻿using Jacobi.AdventureBuilder.AdventureModel;

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

        foreach (var passage in passages)
        {
            var commands = OnPassageLinks(passage);
            _builder.AddRoom(Int64.Parse(passage.Id), passage.Name, passage.CleanText, commands);
        }
    }

    private List<AdventureCommandInfo> OnPassageLinks(Passage passage)
    {
        if (passage.Links is null) return [];
        var links = passage.Links;
        var commands = new List<AdventureCommandInfo>(links.Count);

        foreach (var link in links)
        {
            var passageId = LookupPassageId(link.PassageName);
            var description = LookupLinkDescription(passage.Hooks, link.PassageName)
                ?? $"{link.LinkText} ({link.PassageName})";
            var command = AdventureModelBuilder.CreateNavigateCommand(passageId, link.LinkText, description);
            commands.Add(command);
        }

        return commands;
    }

    private static string? LookupLinkDescription(List<Hook> hooks, string passageName)
        => hooks.FirstOrDefault(h => h.HookName == passageName)?.HookText;

    private string LookupPassageId(string passageName)
        => _twineModel!.Passages.First(p => p.Name == passageName).Id;
}
