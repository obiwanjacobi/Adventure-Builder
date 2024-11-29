namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed class GameCommandInfo
{
    public GameCommandInfo(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    [Id(0)]
    public string Id { get; }
    [Id(1)]
    public string Name { get; }
    [Id(2)]
    public string Description { get; }
}

[GenerateSerializer, Immutable]
public sealed class GameCommand
{
    public GameCommand(string id, string kind, string action)
    {
        Id = id;
        Kind = kind;
        Action = action;
    }

    [Id(0)]
    public string Id { get; }
    [Id(1)]
    public string Kind { get; }
    [Id(2)]
    public string Action { get; }
}

[GenerateSerializer, Immutable]
public sealed class GameCommandResult
{
    public GameCommandResult()
    { }

    public GameCommandResult(IPassageGrain passage)
    {
        Passage = passage;
    }

    [Id(0)]
    public IPassageGrain? Passage { get; }
}

public sealed class GameCommandHandler
{
    private readonly IAdventureWorldGrain world;
    private readonly IPlayerGrain player;

    public GameCommandHandler(IAdventureWorldGrain world, IPlayerGrain player)
    {
        this.world = world;
        this.player = player;
    }

    public Task<GameCommandResult> ExecuteAsync(GameCommand command)
    {
        return command.Kind switch
        {
            "nav-passage" => ExecuteNavigatePassage(command),
            _ => Task.FromResult(new GameCommandResult()),
        };
    }

    private async Task<GameCommandResult> ExecuteNavigatePassage(GameCommand command)
    {
        var targetPassageId = ParsePassageId(command.Action);
        var passageGrain = await this.world.GetPassage(targetPassageId);
        await this.player.EnterPassage(passageGrain);
        return new GameCommandResult(passageGrain);

        static long ParsePassageId(string navPassageAction)
        {
            var index = navPassageAction.LastIndexOf(':');
            return Int64.Parse(navPassageAction[(index + 1)..]);
        }
    }
}