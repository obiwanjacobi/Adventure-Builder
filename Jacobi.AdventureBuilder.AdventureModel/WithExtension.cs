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

    public static IReadOnlyList<AdventureLinkInfo> Add(this IReadOnlyList<AdventureLinkInfo> links, AdventureLinkInfo link)
        => new List<AdventureLinkInfo>(links)
        {
            link
        };
}
