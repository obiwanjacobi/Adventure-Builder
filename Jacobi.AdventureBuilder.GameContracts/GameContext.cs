namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public record class GameContext
{
    public GameContext(IWorldGrain world)
    {
        World = world;
    }

    public GameContext(IWorldGrain world, IPlayerGrain player)
    {
        World = world;
        Player = player;
    }

    public GameContext(IWorldGrain world, IPassageGrain passage)
    {
        World = world;
        Passage = passage;
    }

    public GameContext(IWorldGrain world, IPlayerGrain player, IPassageGrain passage)
    {
        World = world;
        Player = player;
        Passage = passage;
    }

    [Id(0)]
    public IWorldGrain World { get; }
    [Id(1)]
    public IPlayerGrain? Player { get; }
    [Id(2)]
    public IPassageGrain? Passage { get; }
}
