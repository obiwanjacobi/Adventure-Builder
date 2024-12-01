using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PassageGrainState
{
    public AdventurePassageInfo? PassageInfo { get; set; }
}

public sealed class PassageGrain : Grain<PassageGrainState>, IPassageGrain
{
    public Task<AdventurePassageInfo> PassageInfo()
    {
        Debug.Assert(this.State.PassageInfo is not null);
        return Task.FromResult(this.State.PassageInfo);
    }

    public async Task<bool> Load(AdventurePassageInfo passageInfo)
    {
        if (this.State.PassageInfo is not null)
            return false;

        this.State.PassageInfo = passageInfo;
        await WriteStateAsync();
        return true;
    }

    public Task<string> Name()
    {
        ThrowIfUninitialized();
        return Task.FromResult(this.State.PassageInfo!.Name);
    }

    public Task<string> Description()
    {
        ThrowIfUninitialized();
        return Task.FromResult(this.State.PassageInfo!.Description);
    }

    public Task<IReadOnlyList<GameCommandInfo>> CommandInfos()
    {
        ThrowIfUninitialized();
        var commands = this.State.PassageInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Name, cmd.Description))
                .ToList();

        return Task.FromResult((IReadOnlyList<GameCommandInfo>)commands);
    }

    public Task<GameCommand> GetCommand(string commandId)
    {
        ThrowIfUninitialized();
        var commandInfo = this.State.PassageInfo!.Commands
            .Find(cmd => cmd.Id == commandId)
            .Single();
        var command = new GameCommand(commandId, commandInfo.Kind, commandInfo.Action);
        return Task.FromResult(command);
    }

    public Task<IReadOnlyList<GameExtraInfo>> Extras()
    {
        ThrowIfUninitialized();
        var extras = this.State.PassageInfo!.Extras
            .Map(e => new GameExtraInfo { Name = e.Name, Description = e.Description })
            .ToList();
        return Task.FromResult((IReadOnlyList<GameExtraInfo>)extras);
    }

    private void ThrowIfUninitialized()
    {
        if (this.State.PassageInfo is null)
            throw new InvalidOperationException(
                "Uninitialized Passage grain. The Passage info was not loaded.");
    }
}
