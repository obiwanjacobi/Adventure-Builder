using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureCommandInfo
{
    public required string Id { get; init; }
    public required string Kind { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Action { get; init; }
}
