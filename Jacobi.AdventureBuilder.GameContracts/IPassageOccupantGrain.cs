namespace Jacobi.AdventureBuilder.GameContracts;

public interface IPassageOccupantGrain
{
    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();
    [Alias("GotoPassage")]
    Task GotoPassage(GameContext context, IPassageGrain passage);
}
