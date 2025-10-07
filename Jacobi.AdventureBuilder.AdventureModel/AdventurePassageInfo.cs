using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventurePassageInfo
{
    [Id(1)]
    public required long Id { get; init; }
    [Id(2)]
    public required string Name { get; init; }
    [Id(3)]
    public required string Description { get; init; }

    [Id(4)]
    public required IReadOnlyList<AdventureLinkInfo> LinkedPassages { get; init; }
}
