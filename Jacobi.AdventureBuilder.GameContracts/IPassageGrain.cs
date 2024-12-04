using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPassageGrain")]
public interface IPassageGrain : IGrainWithStringKey
{
    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();

    [Alias("CommandInfos")]
    Task<IReadOnlyList<GameCommandInfo>> CommandInfos();
    [Alias("GetCommand")]
    Task<GameCommand> GetCommand(string commandId);

    [Alias("AddExtraInfo")]
    Task<long> AddExtraInfo(AdventureExtraInfo extraInfo);
    [Alias("RemoveExtraInfo")]
    Task RemoveExtraInfo(long extraId);
    [Alias("Extras")]
    Task<IReadOnlyList<GameExtraInfo>> Extras();
}
