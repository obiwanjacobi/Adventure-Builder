using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventurePropertyInfo
{
    public required string Name { get; init; }
    public required string? Value { get; init; }
}