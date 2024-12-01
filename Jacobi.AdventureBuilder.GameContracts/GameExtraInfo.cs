namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed class GameExtraInfo
{
    [Id(0)]
    public required string Name { get; init; }
    [Id(1)]
    public required string Description { get; init; }
}