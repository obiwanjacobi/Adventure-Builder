using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IRoomGrain")]
public interface IRoomGrain : IGrainWithIntegerKey
{
    [Alias("Load")]
    Task<bool> Load(AdventureRoomInfo room);

    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();

    Task<IReadOnlyCollection<GameCommandInfo>> CommandInfos();
    Task<GameCommand> GetCommand(string commandId);
}
