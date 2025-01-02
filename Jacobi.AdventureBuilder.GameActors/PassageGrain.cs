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
    public List<GameCommand> Commands { get; set; } = [];
}

public sealed class PassageGrain : Grain<PassageGrainState>, IPassageGrain
{
    private readonly IAdventureClient _client;
    private readonly INotifyPassage _notify;

    public PassageGrain(IAdventureClient client, INotifyPassage notify)
    {
        _client = client;
        _notify = notify;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = PassageKey.Parse(this.GetPrimaryKeyString());
            State.PassageInfo = await _client.GetAdventurePassageAsync(key.WorldKey.WorldId, key.PassageId);

            State.Commands = State.PassageInfo!.LinkedPassages
                .Map(cmd => new GameCommand(cmd.PassageId.ToString(), GameCommands.NavigatePassage,
                    cmd.Name, cmd.Description,
                    new GameCommandAction(GameCommands.NavigatePassage, cmd.PassageId).ToString()))
                .ToList();

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

    public Task<IReadOnlyList<GameCommand>> Commands()
    {
        return Task.FromResult((IReadOnlyList<GameCommand>)State.Commands);
    }

    public Task<GameCommand> GetCommand(string commandId)
    {
        var command = State.Commands.Find(cmd => cmd.Id == commandId)
            ?? throw new ArgumentException($"This passage '{this.GetPrimaryKeyString()}' does not have a command '{commandId}'.", nameof(commandId));

        return Task.FromResult(command);
    }

    public async Task Enter(string amInPassageKey)
    {
        if (State.OccupantKeys.Contains(amInPassageKey))
            throw new InvalidOperationException($"There is already a '{amInPassageKey}' in this passage.");

        State.OccupantKeys.Add(amInPassageKey);
        await WriteStateAsync();
        await _notify.NotifyPassageEnter(this.GetPrimaryKeyString(), amInPassageKey);
    }

    public async Task Exit(string amInPassageKey)
    {
        State.OccupantKeys.Remove(amInPassageKey);
        await WriteStateAsync();
        await _notify.NotifyPassageExit(this.GetPrimaryKeyString(), amInPassageKey);
    }

    public Task<IReadOnlyList<string>> Occupants()
    {
        return Task.FromResult((IReadOnlyList<string>)State.OccupantKeys);
    }
}
