namespace Jacobi.AdventureBuilder.GameContracts;

public interface IAmInPassage
{
    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();
    [Alias("EnterPassage")]
    Task EnterPassage(IPassageGrain passage);
}
