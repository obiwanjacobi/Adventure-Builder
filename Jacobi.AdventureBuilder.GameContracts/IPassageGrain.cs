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

    [ReadOnly]
    [Alias("Links")]
    Task<IReadOnlyList<PassageLinkInfo>> Links();

    [ReadOnly]
    [Alias("Occupants")]
    Task<IReadOnlyList<string>> Occupants();

    [Alias("Commands")]
    Task<IReadOnlyList<GameCommand>> Commands(IPlayerGrain? player);

    [Alias("Enter")]
    Task Enter(GameContext context, string occupantKey);

    [Alias("Exit")]
    Task Exit(GameContext context, string occupantKey);

    Task Subscribe(IPassageEvents subscriber, string subscriberKey);

    Task Unsubscribe(string subscriberKey);
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
