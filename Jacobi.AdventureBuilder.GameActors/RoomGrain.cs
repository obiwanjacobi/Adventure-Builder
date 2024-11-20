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

    private void ThrowIfUninitialized()
    {
        if (this.roomInfo is null)
            throw new InvalidOperationException(
                "Uninitialized Room grain. The Room info was not loaded.");
    }
}
