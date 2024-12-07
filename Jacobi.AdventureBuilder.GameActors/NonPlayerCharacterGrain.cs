﻿using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public class NonPlayerCharacterGrainState : AmInPassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventureNonPlayerCharacterInfo? NpcInfo { get; set; }
}

public sealed class NonPlayerCharacterGrain : AmInPassageGrain<NonPlayerCharacterGrainState>, INonPlayerCharacterGrain
{
    private readonly IAdventureClient _client;

    public NonPlayerCharacterGrain(IAdventureClient client)
        => _client = client;

    protected override string Name
        => State.NpcInfo!.Name;
    protected override string Description
        => State.NpcInfo!.Description;

    public async override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = NonPlayerCharacterKey.Parse(this.GetPrimaryKeyString());
            State.NpcInfo = await _client.GetAdventureNonPlayerCharacter(key.WorldKey.WorldId, key.NpcId, cancellationToken);
        }

        await base.OnActivateAsync(cancellationToken);
    }
}
