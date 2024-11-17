using System.Diagnostics;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class RoomGrain : Grain, IRoomGrain
{
    private AdventureRoomInfo? roomInfo;

    public ValueTask<AdventureRoomInfo> RoomInfo()
    {
        Debug.Assert(this.roomInfo is not null);
        return ValueTask.FromResult(this.roomInfo);
    }

    public ValueTask Load(AdventureRoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        return ValueTask.CompletedTask;
    }
}
