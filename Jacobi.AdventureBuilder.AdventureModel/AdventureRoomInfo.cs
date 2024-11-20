using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureRoomInfo
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}