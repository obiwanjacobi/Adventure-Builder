﻿using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IWorldGrain")]
public interface IWorldGrain : IGrainWithStringKey
{
    [ReadOnly]
    [Alias("Name")]
    Task<string> Name();

    [Alias("Start")]
    Task<IPassageGrain> Start(IPlayerGrain player);

    [Alias("GetPassage")]
    Task<IPassageGrain> GetPassage(long passageId);
}
