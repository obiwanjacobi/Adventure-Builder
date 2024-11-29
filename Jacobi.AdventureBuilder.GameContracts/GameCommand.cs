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

    public GameCommandResult(IPassageGrain room)
    {
        Room = room;
    }

    [Id(0)]
    public IPassageGrain? Room { get; }
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
            "nav-room" => ExecuteNavigateRoom(command),
            _ => Task.FromResult(new GameCommandResult()),
        };
    }

    private async Task<GameCommandResult> ExecuteNavigateRoom(GameCommand command)
    {
        var targetRoomId = ParseRoomId(command.Action);
        var roomGrain = await this.world.GetPassage(targetRoomId);
        await this.player.EnterPassage(roomGrain);
        return new GameCommandResult(roomGrain);

        static long ParseRoomId(string navRoomAction)
        {
            var index = navRoomAction.LastIndexOf(':');
            return Int64.Parse(navRoomAction[(index + 1)..]);
        }
    }
}