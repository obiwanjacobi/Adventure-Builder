using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class RoomGrainState
{
    public AdventureRoomInfo? RoomInfo { get; set; }
}

public sealed class RoomGrain : Grain<RoomGrainState>, IRoomGrain
{
    public Task<AdventureRoomInfo> RoomInfo()
    {
        Debug.Assert(this.State.RoomInfo is not null);
        return Task.FromResult(this.State.RoomInfo);
    }

    public async Task<bool> Load(AdventureRoomInfo roomInfo)
    {
        if (this.State.RoomInfo is not null)
            return false;

        this.State.RoomInfo = roomInfo;
        await WriteStateAsync();
        return true;
    }

    public Task<string> Name()
    {
        ThrowIfUninitialized();
        return Task.FromResult(this.State.RoomInfo!.Name);
    }

    public Task<string> Description()
    {
        ThrowIfUninitialized();
        return Task.FromResult(this.State.RoomInfo!.Description);
    }

    public Task<IReadOnlyCollection<GameCommandInfo>> CommandInfos()
    {
        ThrowIfUninitialized();
        return Task.FromResult(
            (IReadOnlyCollection<GameCommandInfo>)this.State.RoomInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Name, cmd.Description))
                .ToList()
        );
    }

    public Task<GameCommand> GetCommand(string commandId)
    {
        ThrowIfUninitialized();
        var commandInfo = this.State.RoomInfo!.Commands
            .Find(cmd => cmd.Id == commandId)
            .Single();
        var command = new GameCommand(commandId, commandInfo.Kind, commandInfo.Action);
        return Task.FromResult(command);
    }

    private void ThrowIfUninitialized()
    {
        if (this.State.RoomInfo is null)
            throw new InvalidOperationException(
                "Uninitialized Room grain. The Room info was not loaded.");
    }
}
