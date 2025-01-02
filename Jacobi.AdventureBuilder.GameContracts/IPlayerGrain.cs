namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPlayerGrain")]
public interface IPlayerGrain : IAmInPassage, IGrainWithStringKey
{
    [Alias("Play")]
    Task<GameCommandResult> Play(IWorldGrain world, IPassageGrain passage, GameCommand command);

    [Alias("Log")]
    Task<IPlayerLogGrain> Log();

    [Alias("Inventory")]
    Task<IPlayerInventoryGrain> Inventory();
}

public interface IPlayerLogGrain : IGrainWithStringKey
{
    Task Clear();
    Task<IReadOnlyList<PlayerLogLine>> Lines();

    Task AddLine(GameCommand command);
    Task AddLine(IPassageGrain passage, string playerKey);
    Task UpdateLine(IPassageGrain passage, string playerKey);
}

[GenerateSerializer, Immutable]
public sealed class PlayerLogLine
{
    public PlayerLogLine(long id, PlayerLogLineKind kind, string title, string description,
        string? grainKey = null, IReadOnlyList<PlayerLogLine>? subLines = null)
    {
        Id = id;
        Kind = kind;
        Title = title;
        Description = description;
        GrainKey = grainKey;
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
    public string? GrainKey { get; }
    [Id(5)]
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
