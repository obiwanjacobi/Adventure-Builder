using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventurePassageInfo? PassageInfo { get; set; }
    public List<string> OccupantKeys { get; } = [];
}

public sealed class PassageGrain : Grain<PassageGrainState>, IPassageGrain
{
    private readonly IAdventureClient _client;

    public PassageGrain(IAdventureClient client)
        => _client = client;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = PassageKey.Parse(this.GetPrimaryKeyString());
            State.PassageInfo = await _client.GetAdventurePassageAsync(key.WorldKey.WorldId, key.PassageId);

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

    public Task<IReadOnlyList<GameCommandInfo>> CommandInfos()
    {
        var commands = State.PassageInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Kind, cmd.Name, cmd.Description))
                .ToList();

        return Task.FromResult((IReadOnlyList<GameCommandInfo>)commands);
    }

    public Task<GameCommand> GetCommand(string commandId)
    {
        var commandInfo = State.PassageInfo!.Commands
            .Find(cmd => cmd.Id == commandId)
            .Single();
        var command = new GameCommand(commandId, commandInfo.Kind, commandInfo.Action);
        return Task.FromResult(command);
    }

    public async Task Enter(string amInPassageKey)
    {
        if (State.OccupantKeys.Contains(amInPassageKey))
            throw new InvalidOperationException($"There is already a '{amInPassageKey}' in this passage.");

        State.OccupantKeys.Add(amInPassageKey);
        await WriteStateAsync();
    }

    public Task Exit(string amInPassageKey)
    {
        State.OccupantKeys.Remove(amInPassageKey);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> Occupants()
    {
        return Task.FromResult((IReadOnlyList<string>)State.OccupantKeys);
    }
}
