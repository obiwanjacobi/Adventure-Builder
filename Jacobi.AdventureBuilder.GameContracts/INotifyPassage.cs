namespace Jacobi.AdventureBuilder.GameContracts;

public interface INotifyPassage
{
    Task NotifyPassageEnter(string passageKey, string characterKey);
    Task NotifyPassageExit(string passageKey, string characterKey);
}
