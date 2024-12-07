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
        => Task.FromResult(State.PassageInfo!.Name);

    public Task<string> Description()
        => Task.FromResult(State.PassageInfo!.Description);

    public Task<IReadOnlyList<GameCommandInfo>> CommandInfos()
    {
        var commands = State.PassageInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Name, cmd.Description))
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

    public async Task<long> AddExtraInfo(AdventureExtraInfo extraInfo)
    {
        var key = PassageKey.Parse(this.GetPrimaryKeyString());
        if (extraInfo.PassageId != 0 && key.PassageId != extraInfo.PassageId)
            throw new ArgumentException($"The ExtraInfo does not belong to this Passage: {extraInfo}", nameof(extraInfo));

        var extraId = State.PassageInfo!.Extras.Count;
        State.PassageInfo = State.PassageInfo!.Add([extraInfo]);
        await WriteStateAsync();
        return extraId;
    }

    public Task RemoveExtraInfo(long extraId)
    {
        if (extraId < 0 || extraId >= State.PassageInfo!.Extras.Count)
            throw new ArgumentException($"Invalid ExtraInfo Id: {extraId}", nameof(extraId));

        var extraInfo = State.PassageInfo!.Extras[(int)extraId]!;
        State.PassageInfo = State.PassageInfo!.Remove(extraInfo);
        return WriteStateAsync();
    }

    public Task<IReadOnlyList<GameExtraInfo>> Extras()
    {
        var extras = State.PassageInfo!.Extras
            .Map(e => new GameExtraInfo { Name = e.Name, Description = e.Description })
            .ToList();
        return Task.FromResult((IReadOnlyList<GameExtraInfo>)extras);
    }
}
