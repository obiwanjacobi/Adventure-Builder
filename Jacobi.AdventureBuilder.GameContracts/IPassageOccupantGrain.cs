using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameContracts;

public interface IPassageOccupantGrain
{
    [ReadOnly]
    [Alias("Name")]
    Task<string> Name();

    [ReadOnly]
    [Alias("Description")]
    Task<string> Description();

    [Alias("GotoPassage")]
    Task GotoPassage(GameContext context, IPassageGrain passage);
}
