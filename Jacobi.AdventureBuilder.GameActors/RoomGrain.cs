using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class RoomGrain : Grain, IRoomGrain
{
    private AdventureRoomInfo? roomInfo;

    public Task<AdventureRoomInfo> RoomInfo()
    {
        Debug.Assert(this.roomInfo is not null);
        return Task.FromResult(this.roomInfo);
    }

    public Task<bool> Load(AdventureRoomInfo roomInfo)
    {
        if (this.roomInfo is not null)
            return Task.FromResult(false);

        this.roomInfo = roomInfo;
        return Task.FromResult(true);
    }

    public Task<string> Name()
    {
        ThrowIfUninitialized();
        return Task.FromResult(this.roomInfo!.Name);
    }

    public Task<string> Description()
    {
        ThrowIfUninitialized();
        return Task.FromResult(this.roomInfo!.Description);
    }

    public Task<IReadOnlyCollection<GameCommandInfo>> CommandInfos()
    {
        ThrowIfUninitialized();
        return Task.FromResult(
            (IReadOnlyCollection<GameCommandInfo>)this.roomInfo!.Commands
                .Map(cmd => new GameCommandInfo(cmd.Id, cmd.Name, cmd.Description))
                .ToList()
        );
    }

    public Task<GameCommand> GetCommand(string commandId)
    {
        ThrowIfUninitialized();
        var commandInfo = this.roomInfo!.Commands
            .Find(cmd => cmd.Id == commandId)
            .Single();
        var command = new GameCommand(commandId, commandInfo.Kind, commandInfo.Action);
        return Task.FromResult(command);
    }

    private void ThrowIfUninitialized()
    {
        if (this.roomInfo is null)
            throw new InvalidOperationException(
                "Uninitialized Room grain. The Room info was not loaded.");
    }
}
