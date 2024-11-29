using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPassageGrain")]
public interface IPassageGrain : IGrainWithIntegerKey
{
    [Alias("Load")]
    Task<bool> Load(AdventurePassageInfo passage);

    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();

    Task<IReadOnlyCollection<GameCommandInfo>> CommandInfos();
    Task<GameCommand> GetCommand(string commandId);
}
