using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureLinkInfo
{
    public required long PassageId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
