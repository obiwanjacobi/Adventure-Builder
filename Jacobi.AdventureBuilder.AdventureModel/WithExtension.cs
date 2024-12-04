﻿namespace Jacobi.AdventureBuilder.AdventureModel;

public static class WithExtensions
{
    public static AdventureWorldInfo With(this AdventureWorldInfo world, IReadOnlyList<AdventurePassageInfo> passages)
    {
        return new AdventureWorldInfo
        {
            Id = world.Id,
            Name = world.Name,
            NonPlayerCharacters = world.NonPlayerCharacters,
            Passages = passages
        };
    }

    public static AdventureWorldInfo Replace(this AdventureWorldInfo world, IList<AdventurePassageInfo> passages)
    {
        var newPassages = new List<AdventurePassageInfo>(world.Passages.Count);

        foreach (var passage in world.Passages)
        {
            var passageFound = passages.SingleOrDefault(p => passage.Id == p.Id);
            if (passageFound is not null)
                newPassages.Add(passageFound);
            else
                newPassages.Add(passage);
        }

        return world.With(newPassages);
    }

    public static AdventurePassageInfo With(this AdventurePassageInfo passage, IEnumerable<AdventureExtraInfo> extra)
        => new()
        {
            Id = passage.Id,
            Name = passage.Name,
            Description = passage.Description,
            Commands = passage.Commands,
            Extras = [.. passage.Extras, .. extra]
        };

    public static IReadOnlyList<AdventureCommandInfo> Add(this IReadOnlyList<AdventureCommandInfo> commands, AdventureCommandInfo command)
        => new List<AdventureCommandInfo>(commands)
        {
            command
        };
}