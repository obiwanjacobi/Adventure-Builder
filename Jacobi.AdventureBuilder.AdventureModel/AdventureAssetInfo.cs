using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureAssetInfo
{
    [Id(1)]
    public required long Id { get; init; }
    [Id(2)]
    public required string Name { get; init; }
    [Id(3)]
    public required string Description { get; init; }

    [Id(4)]
    public required IReadOnlyList<long> LinkedPassageIds { get; init; }
    [Id(5)]
    public required IReadOnlyList<AdventurePropertyInfo> Properties { get; init; }
}
