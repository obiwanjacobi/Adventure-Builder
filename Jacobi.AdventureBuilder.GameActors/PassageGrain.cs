using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventurePassageInfo? PassageInfo { get; set; }
    public List<string> OccupantKeys { get; } = [];
}

[Reentrant]
public sealed class PassageGrain : Grain<PassageGrainState>, IPassageGrain
{
    private readonly IAdventureClient _client;
    private readonly GameCommandExecuter _executer;

    public PassageGrain(IAdventureClient client, GameCommandExecuter executer)
    {
        _client = client;
        _executer = executer;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = PassageKey.Parse(this.GetPrimaryKeyString());
            State.PassageInfo = await _client.GetAdventurePassageAsync(key.WorldKey.WorldId, key.PassageId, cancellationToken);

            await WriteStateAsync();
        }
        await base.OnActivateAsync(cancellationToken);
    }

    public Task<AdventurePassageInfo> PassageInfo()
    {
        Debug.Assert(State.PassageInfo is not null);
        return Task.FromResult(State.PassageInfo);
    }

    public Task<string> Name()
        => Task.FromResult(State.PassageInfo!.Name);

    public Task<string> Description()
        => Task.FromResult(State.PassageInfo!.Description);

    public async Task<IReadOnlyList<GameCommand>> Commands(IPlayerGrain? player)
    {
        var key = PassageKey.Parse(this.GetPrimaryKeyString());
        var world = GrainFactory.GetGrain<IWorldGrain>(key.WorldKey);
        var commands = await _executer.ProvideCommands(world, this, player);
        return commands;
    }

    public Task<IReadOnlyList<PassageLinkInfo>> Links()
    {
        return Task.FromResult(
            (IReadOnlyList<PassageLinkInfo>)State.PassageInfo!.LinkedPassages
                .Map(lnk => new PassageLinkInfo(lnk.PassageId, lnk.Name, lnk.Description))
                .ToList()
        );
    }

    public async Task Enter(GameContext context, string occupantKey)
    {
        if (State.OccupantKeys.Contains(occupantKey))
            throw new InvalidOperationException($"There is already a '{occupantKey}' in this passage.");

        State.OccupantKeys.Add(occupantKey);
        await WriteStateAsync();

        var key = this.GetPrimaryKeyString();
        var notify = GrainFactory.GetNotifyPassage(key);
        await notify.NotifyPassageEnter(context, key, occupantKey);
    }

    public async Task Exit(GameContext context, string occupantKey)
    {
        State.OccupantKeys.Remove(occupantKey);
        await WriteStateAsync();

        var key = this.GetPrimaryKeyString();
        var notify = GrainFactory.GetNotifyPassage(key);
        await notify.NotifyPassageExit(context, key, occupantKey);
    }

    public Task<IReadOnlyList<string>> Occupants()
    {
        return Task.FromResult((IReadOnlyList<string>)State.OccupantKeys);
    }
}
