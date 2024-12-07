namespace Jacobi.AdventureBuilder.GameContracts;

public interface IAmInPassage
{
    [Alias("EnterPassage")]
    Task EnterPassage(IPassageGrain passage);
}
