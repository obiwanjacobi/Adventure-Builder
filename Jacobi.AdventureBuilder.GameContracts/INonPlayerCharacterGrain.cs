namespace Jacobi.AdventureBuilder.GameContracts;

public interface INonPlayerCharacterGrain : IGrainWithStringKey
{
    [Alias("EnterPassage")]
    Task EnterPassage(IPassageGrain passage);
}
