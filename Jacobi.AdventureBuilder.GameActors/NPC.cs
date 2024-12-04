using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameActors;

internal static class NPC
{
    public static IReadOnlyList<AdventureExtraInfo> SpawnNPCs(AdventureWorldInfo world)
    {
        var spawnings = new List<AdventureExtraInfo>(world.NonPlayerCharacters.Count);

        foreach (var npc in world.NonPlayerCharacters)
        {
            AdventurePassageInfo passage = SpawnInPassage(world, npc);
            spawnings.Add(new AdventureExtraInfo
            {
                PassageId = passage.Id,
                Name = npc.Name,
                Description = npc.Description,
            });
        }

        return spawnings;
    }

    public static AdventurePassageInfo SpawnInPassage(AdventureWorldInfo world, AdventureNonPlayerCharacterInfo npc)
    {
        AdventurePassageInfo? passage = null;

        if (npc.LinkedPassageIds.Count > 0)
        {
            var linkedPassageIndex = Random.Shared.Next(npc.LinkedPassageIds.Count);
            passage = world.Passages[linkedPassageIndex];
        }
        else
        {
            var passageIndex = Random.Shared.Next(world.Passages.Count);
            passage = world.Passages[passageIndex];
        }

        return passage;
    }
}