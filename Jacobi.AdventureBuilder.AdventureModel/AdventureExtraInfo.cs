﻿using Orleans;

namespace Jacobi.AdventureBuilder.AdventureModel;

[GenerateSerializer, Immutable]
public sealed class AdventureExtraInfo
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}