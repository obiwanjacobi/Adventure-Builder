using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerLogGrain")]
public interface IPlayerLogGrain : IGrainWithStringKey
{
    Task Clear();
    [ReadOnly]
    Task<IReadOnlyList<PlayerLogLine>> Lines();

    Task AddLine(GameCommand command);
    Task AddLine(IPassageGrain passage, GameCommand? command = null);
    Task UpdateLine(IPassageGrain passage, GameCommand? command = null);
}

[GenerateSerializer, Immutable]
public sealed class PlayerLogLine
{
    public PlayerLogLine(long id, PlayerLogLineKind kind, string title, string description,
        string? commandKind = null, string? subject = null, IReadOnlyList<PlayerLogLine>? subLines = null)
    {
        Id = id;
        Kind = kind;
        Title = title;
        Description = description;
        CommandKind = commandKind;
        Subject = subject;
        SubLines = subLines;
    }

    [Id(0)]
    public long Id { get; }
    [Id(1)]
    public PlayerLogLineKind Kind { get; }
    [Id(2)]
    public string Title { get; }
    [Id(3)]
    public string Description { get; }
    [Id(4)]
    public string? CommandKind { get; }
    [Id(5)]
    public string? Subject { get; }
    [Id(6)]
    public IReadOnlyList<PlayerLogLine>? SubLines { get; }
}

public enum PlayerLogLineKind
{
    None,
    Passage,
    Command,
    // sub types
    Player,
    NonPlayerCharacter,
    Asset
}
