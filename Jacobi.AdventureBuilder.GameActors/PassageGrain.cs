using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventurePassageInfo? PassageInfo { get; set; }
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
    {
        ThrowIfNotLoaded();
        return Task.FromResult(State.PassageInfo!.Name);
    }

    public Task<string> Description()
    {
        ThrowIfNotLoaded();
        return Task.FromResult(State.PassageInfo!.Description);
    }

    public Task<IReadOnlyList<GameCommandInfo>> CommandInfos()
    {
        ThrowIfNotLoaded();
        var commands = State.PassageInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Name, cmd.Description))
                .ToList();

        return Task.FromResult((IReadOnlyList<GameCommandInfo>)commands);
    }

    public Task<GameCommand> GetCommand(string commandId)
    {
        ThrowIfNotLoaded();
        var commandInfo = State.PassageInfo!.Commands
            .Find(cmd => cmd.Id == commandId)
            .Single();
        var command = new GameCommand(commandId, commandInfo.Kind, commandInfo.Action);
        return Task.FromResult(command);
    }

    public Task AddExtraInfo(AdventureExtraInfo extraInfo)
    {
        ThrowIfNotLoaded();
        var key = PassageKey.Parse(this.GetPrimaryKeyString());
        if (key.PassageId != extraInfo.PassageId)
            throw new ArgumentException($"The ExtraInfo does not belong to this Passage: {extraInfo}", nameof(extraInfo));

        State.PassageInfo = State.PassageInfo!.Add([extraInfo]);
        return WriteStateAsync();
    }

    public Task<IReadOnlyList<GameExtraInfo>> Extras()
    {
        ThrowIfNotLoaded();
        var extras = State.PassageInfo!.Extras
            .Map(e => new GameExtraInfo { Name = e.Name, Description = e.Description })
            .ToList();
        return Task.FromResult((IReadOnlyList<GameExtraInfo>)extras);
    }

    private void ThrowIfNotLoaded()
    {
        if (State.PassageInfo is null)
            throw new InvalidOperationException(
                "Uninitialized Passage grain. The Passage info was not loaded.");
    }
}
