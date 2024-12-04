namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPassageGrain")]
public interface IPassageGrain : IGrainWithStringKey
{
    //[Alias("Load")]
    //Task<bool> Load(AdventurePassageInfo passage);

    [Alias("Name")]
    Task<string> Name();
    [Alias("Description")]
    Task<string> Description();

    [Alias("CommandInfos")]
    Task<IReadOnlyList<GameCommandInfo>> CommandInfos();
    [Alias("GetCommand")]
    Task<GameCommand> GetCommand(string commandId);

    [Alias("Extras")]
    Task<IReadOnlyList<GameExtraInfo>> Extras();
}
