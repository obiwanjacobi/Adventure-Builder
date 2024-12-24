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

    [Alias("Enter")]
    Task Enter(string amInPassageKey);
    [Alias("Exit")]
    Task Exit(string amInPassageKey);
    [Alias("Occupants")]
    Task<IReadOnlyList<string>> Occupants();
}
