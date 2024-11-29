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

    public Task<IReadOnlyCollection<GameCommandInfo>> CommandInfos()
    {
        ThrowIfUninitialized();
        return Task.FromResult(
            (IReadOnlyCollection<GameCommandInfo>)this.State.PassageInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Name, cmd.Description))
                .ToList()
        );
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

    private void ThrowIfUninitialized()
    {
        if (this.State.PassageInfo is null)
            throw new InvalidOperationException(
                "Uninitialized Room grain. The Room info was not loaded.");
    }
}
