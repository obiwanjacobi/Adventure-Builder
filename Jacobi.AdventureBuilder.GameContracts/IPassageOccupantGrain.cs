namespace Jacobi.AdventureBuilder.GameContracts;

public interface IPassageOccupantGrain
{
    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();
    [Alias("EnterPassage")]
    Task EnterPassage(GameContext context, IPassageGrain passage);
}
