namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IWorldManagerGrain")]
public interface IWorldManagerGrain : IGrainWithGuidKey
{
    [Alias("CreateWorld")]
    Task<IWorldGrain> CreateNewWorld(string worldId, string name);
}
