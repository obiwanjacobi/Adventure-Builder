using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventurePropertyInfo
{
    [Id(1)]
    public required string Name { get; init; }
    [Id(2)]
    public required string? Value { get; init; }
}
