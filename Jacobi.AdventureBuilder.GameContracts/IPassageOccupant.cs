namespace Jacobi.AdventureBuilder.GameContracts;

public interface IPassageOccupant
{
    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();
    [Alias("EnterPassage")]
    Task EnterPassage(IPassageGrain passage);
}
