﻿using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.Twine;

internal sealed class AdventureModelBuilder
{
    private readonly List<AdventurePassageInfo> _passages = [];
    private readonly List<AdventureNonPlayerCharacterInfo> _npcs = [];

    public AdventurePassageInfo AddPassage(long id, string name, string description, IReadOnlyCollection<AdventureCommandInfo> commands)
    {
        var passage = new AdventurePassageInfo()
        {
            Id = id,
            Name = name,
            Description = description,
            Commands = commands,
        };

        _passages.Add(passage);
        return passage;
    }

    public static AdventureCommandInfo CreateNavigateCommand(string id, string name, string description)
    {
        return new AdventureCommandInfo
        {
            Id = id,
            Name = name,
            Description = description,
            Kind = "nav-passage",
            Action = $"nav:passage:{id}",
        };
    }

    public AdventureWorldInfo Build(string id, string name)
    {
        var world = new AdventureWorldInfo
        {
            Id = id,
            Name = name,
            Passages = _passages.ToList(),
            NonPlayerCharacters = _npcs.ToList()
        };

        _passages.Clear();
        _npcs.Clear();

        return world;
    }

    public AdventureNonPlayerCharacterInfo AddNonPlayerCharacter(long id, string name, string description, IReadOnlyCollection<AdventurePassageInfo> linkedPassages)
    {
        var npc = new AdventureNonPlayerCharacterInfo
        {
            Id = id,
            Name = name,
            Description = description,
            LinkedPassages = linkedPassages
        };

        _npcs.Add(npc);
        return npc;
    }
}
