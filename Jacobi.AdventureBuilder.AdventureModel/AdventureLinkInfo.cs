using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureLinkInfo
{
    [Id(1)]
    public required long PassageId { get; init; }
    [Id(2)]
    public required string Name { get; init; }
    [Id(3)]
    public required string Description { get; init; }
}
