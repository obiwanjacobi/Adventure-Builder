using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameActors;

internal static class NPC
{
    public static AdventureWorldInfo SpawnNPCs(AdventureWorldInfo world)
    {
        var passageReplacements = new List<AdventurePassageInfo>(world.NonPlayerCharacters.Count);

        foreach (var npc in world.NonPlayerCharacters)
        {
            AdventurePassageInfo? passage = null;

            if (npc.LinkedPassages.Count > 0)
            {
                var linkedPassageIndex = Random.Shared.Next(npc.LinkedPassages.Count);
                passage = world.Passages[linkedPassageIndex];
            }
            else
            {
                var passageIndex = Random.Shared.Next(world.Passages.Count);
                passage = world.Passages[passageIndex];
            }

            if (passage is not null)
            {
                // TODO: check if this passage is in replacements already

                var newPassage = SpawnNPCIn(npc, passage);
                passageReplacements.Add(newPassage);
            }
        }

        if (passageReplacements.Count == 0)
            return world;

        return world.Replace(passageReplacements);
    }

    private static AdventurePassageInfo SpawnNPCIn(AdventureNonPlayerCharacterInfo npc, AdventurePassageInfo passage)
    {
        return passage.With([new AdventureExtraInfo
        {
            Name = npc.Name,
            Description = npc.Description
        }]);
    }
}