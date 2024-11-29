using System.Text.Json.Serialization;
using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureWorldInfo
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    [JsonIgnore]
    public AdventurePassageInfo StartPassage => Passages.First();

    public required IReadOnlyCollection<AdventurePassageInfo> Passages { get; init; }

    public required IReadOnlyCollection<AdventureNonPlayerCharacterInfo> NonPlayerCharacters { get; init; }
}
