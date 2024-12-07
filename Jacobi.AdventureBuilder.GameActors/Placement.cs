using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameActors;

internal static class Placement
{
    public static AdventurePassageInfo PlaceInPassage(AdventureWorldInfo world, IReadOnlyList<long> passageIds)
    {
        AdventurePassageInfo? passage = null;

        if (passageIds.Count > 0)
        {
            var passageIndex = Random.Shared.Next(passageIds.Count - 1);
            passage = world.Passages.First(p => p.Id == passageIds[passageIndex]);
        }
        else
        {
            var passageIndex = Random.Shared.Next(world.Passages.Count - 1);
            passage = world.Passages[passageIndex];
        }

        return passage;
    }
}