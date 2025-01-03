using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public interface IGameCommandHandler
{
    // handler
    bool CanHandleCommand(GameCommand command);
    Task<GameCommandResult> HandleCommand(GameCommandContext context, GameCommand command);

    // provder
    Task<IReadOnlyList<GameCommand>> ProvideCommands(GameCommandContext context);
}

public sealed record class GameCommandContext
{
    public GameCommandContext(IWorldGrain world, IPassageGrain passage)
    {
        World = world;
        Passage = passage;
    }

    public GameCommandContext(IWorldGrain world, IPassageGrain passage, IPlayerGrain player)
        : this(world, passage)
    {
        Player = player;
        Issuer = player;
        IssuerKey = player.GetPrimaryKeyString();
    }

    public GameCommandContext(IWorldGrain world, IPassageGrain passage, INonPlayerCharacterGrain npc)
        : this(world, passage)
    {
        Npc = npc;
        Issuer = npc;
        IssuerKey = npc.GetPrimaryKeyString();
    }

    public IWorldGrain World { get; }
    public IPassageGrain Passage { get; }

    public IPlayerGrain? Player { get; }
    public INonPlayerCharacterGrain? Npc { get; }

    public IPassageOccupant? Issuer { get; }
    public string? IssuerKey { get; }
}

public sealed class GameCommandExecuter
{
    private readonly List<IGameCommandHandler> _handlers;

    public GameCommandExecuter(IEnumerable<IGameCommandHandler> handlers)
    {
        _handlers = handlers.ToList();
    }

    public Task<GameCommandResult> ExecuteAsync(IWorldGrain world, IPlayerGrain issuer, IPassageGrain passage, GameCommand command)
    {
        var context = new GameCommandContext(world, passage, issuer);
        return ExecuteAsync(context, command);
    }

    public Task<GameCommandResult> ExecuteAsync(IWorldGrain world, INonPlayerCharacterGrain issuer, IPassageGrain passage, GameCommand command)
    {
        var context = new GameCommandContext(world, passage, issuer);
        return ExecuteAsync(context, command);
    }

    private Task<GameCommandResult> ExecuteAsync(GameCommandContext context, GameCommand command)
    {
        foreach (var handler in _handlers)
        {
            if (handler.CanHandleCommand(command))
                return handler.HandleCommand(context, command);
        }

        return Task.FromResult(new GameCommandResult());
    }

    public async Task<IReadOnlyList<GameCommand>> ProviderCommands(IWorldGrain world, IPassageGrain passage, IPlayerGrain? player = null)
    {
        var commands = new List<GameCommand>();
        var context = player is null
            ? new GameCommandContext(world, passage)
            : new GameCommandContext(world, passage, player);

        foreach (var handler in _handlers)
        {
            commands.AddRange(await handler.ProvideCommands(context));
        }

        return commands;
    }
}

public readonly record struct GameCommandAction
{
    public static GameCommandAction Parse(string actionMoniker)
    {
        var parts = actionMoniker.Split(':');
        return new GameCommandAction(parts[0], Int64.Parse(parts[^1]));
    }

    public GameCommandAction(string kind, long id)
    {
        Kind = kind;
        Id = id;
    }

    public string Kind { get; }
    public long Id { get; }

    public override string ToString()
    {
        return $"{Kind}:{Id}";
    }
}
