using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventurePassageInfo? PassageInfo { get; set; }
    public List<string> OccupantKeys { get; } = [];
}

public sealed class PassageGrain : Grain<PassageGrainState>, IPassageGrain
{
    private readonly IGrainFactory _factory;
    private readonly IAdventureClient _client;
    private readonly GameCommandExecuter _executer;

    public PassageGrain(IGrainFactory factory, IAdventureClient client,
        GameCommandExecuter executer)
    {
        _factory = factory;
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
        var world = _factory.GetGrain<IWorldGrain>(key.WorldKey);
        var commands = await _executer.ProviderCommands(world, this, player);
        return commands;
    }

    public Task<IReadOnlyList<PassageLinkInfo>> Links()
    {
        return Task.FromResult((IReadOnlyList<PassageLinkInfo>)State.PassageInfo!.LinkedPassages
            .Map(lnk => new PassageLinkInfo(lnk.PassageId, lnk.Name, lnk.Description)).ToList());
    }

    public async Task Enter(string occupantKey)
    {
        if (State.OccupantKeys.Contains(occupantKey))
            throw new InvalidOperationException($"There is already a '{occupantKey}' in this passage.");

        State.OccupantKeys.Add(occupantKey);
        await WriteStateAsync();

        var notify = _factory.GetNotifyPassage();
        await notify.NotifyPassageEnter(this.GetPrimaryKeyString(), occupantKey);
    }

    public async Task Exit(string occupantKey)
    {
        State.OccupantKeys.Remove(occupantKey);
        await WriteStateAsync();

        var notify = _factory.GetNotifyPassage();
        await notify.NotifyPassageExit(this.GetPrimaryKeyString(), occupantKey);
    }

    public Task<IReadOnlyList<string>> Occupants()
    {
        return Task.FromResult((IReadOnlyList<string>)State.OccupantKeys);
    }
}
