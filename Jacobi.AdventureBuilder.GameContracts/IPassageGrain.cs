using Orleans.Concurrency;

namespace Jacobi.AdventureBuilder.GameContracts;

[Alias("AdventureBuilder.IPassageGrain")]
public interface IPassageGrain : IGrainWithStringKey
{
    [ReadOnly]
    [Alias("Name")]
    Task<string> Name();
    [ReadOnly]
    [Alias("Description")]
    Task<string> Description();

    [Alias("Commands")]
    Task<IReadOnlyList<GameCommand>> Commands(IPlayerGrain? player);

    [ReadOnly]
    [Alias("Links")]
    Task<IReadOnlyList<PassageLinkInfo>> Links();

    [Alias("Enter")]
    Task Enter(GameContext context, string occupantKey);
    [Alias("Exit")]
    Task Exit(GameContext context, string occupantKey);

    [ReadOnly]
    [Alias("Occupants")]
    Task<IReadOnlyList<string>> Occupants();
}

[GenerateSerializer, Immutable]
public sealed class PassageLinkInfo
{
    public PassageLinkInfo(long passageId, string name, string description)
    {
        PassageId = passageId;
        Name = name;
        Description = description;
    }

    [Id(0)]
    public long PassageId { get; }
    [Id(1)]
    public string Name { get; }
    [Id(2)]
    public string Description { get; }
}
