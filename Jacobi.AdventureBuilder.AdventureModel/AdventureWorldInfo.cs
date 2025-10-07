using System.Text.Json.Serialization;
using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureWorldInfo
{
    [Id(1)]
    public required string Id { get; init; }
    [Id(2)]
    public required string Name { get; init; }

    [JsonIgnore]
    public AdventurePassageInfo StartPassage => Passages.First();

    [Id(3)]
    public required IReadOnlyList<AdventurePassageInfo> Passages { get; init; }
    [Id(4)]
    public required IReadOnlyList<AdventureNonPlayerCharacterInfo> NonPlayerCharacters { get; init; }
    [Id(5)]
    public required IReadOnlyList<AdventureAssetInfo> Assets { get; init; }
}
