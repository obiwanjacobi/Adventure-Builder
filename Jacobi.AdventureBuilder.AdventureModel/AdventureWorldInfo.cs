using System.Text.Json.Serialization;
using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureWorldInfo
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    [JsonIgnore]
    public AdventureRoomInfo StartRoom => Rooms.First();

    public required IReadOnlyCollection<AdventureRoomInfo> Rooms { get; init; }
}
