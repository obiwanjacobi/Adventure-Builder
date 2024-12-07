namespace Jacobi.AdventureBuilder.AdventureModel;

public static class WithExtensions
{
    public static AdventureWorldInfo With(this AdventureWorldInfo world, IReadOnlyList<AdventurePassageInfo> passages)
    {
        return new AdventureWorldInfo
        {
            Id = world.Id,
            Name = world.Name,
            NonPlayerCharacters = world.NonPlayerCharacters,
            Passages = passages,
            Assets = world.Assets,
        };
    }

    public static AdventurePassageInfo Add(this AdventurePassageInfo passage, IEnumerable<AdventureExtraInfo> extra)
        => new()
        {
            Id = passage.Id,
            Name = passage.Name,
            Description = passage.Description,
            Commands = passage.Commands,
            Extras = [.. passage.Extras, .. extra]
        };

    public static AdventurePassageInfo Remove(this AdventurePassageInfo passage, AdventureExtraInfo extraInfo)
    {
        var extras = passage.Extras.ToList();
        extras.Remove(extraInfo);

        return new()
        {
            Id = passage.Id,
            Name = passage.Name,
            Description = passage.Description,
            Commands = passage.Commands,
            Extras = extras
        };
    }

    public static IReadOnlyList<AdventureCommandInfo> Add(this IReadOnlyList<AdventureCommandInfo> commands, AdventureCommandInfo command)
        => new List<AdventureCommandInfo>(commands)
        {
            command
        };
}
