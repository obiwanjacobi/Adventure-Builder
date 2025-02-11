﻿using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventurePassageInfo
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public required IReadOnlyList<AdventureLinkInfo> LinkedPassages { get; init; }
}
