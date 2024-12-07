using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureAssetInfo
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public required IReadOnlyList<long> LinkedPassageIds { get; init; }
    public required IReadOnlyList<AdventurePropertyInfo> Properties { get; init; }
}
