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

public sealed record class GameCommandContext(
    IWorldGrain World, IPassageGrain Passage, IAmInPassage? Issuer = null);

public sealed class GameCommandExecuter
{
    private readonly List<IGameCommandHandler> _handlers;

    public GameCommandExecuter(IEnumerable<IGameCommandHandler> handlers)
    {
        _handlers = handlers.ToList();
    }

    public Task<GameCommandResult> ExecuteAsync(IWorldGrain world, IAmInPassage issuer, IPassageGrain passage, GameCommand command)
    {
        var context = new GameCommandContext(world, passage, issuer);
        foreach (var handler in _handlers)
        {
            if (handler.CanHandleCommand(command))
                return handler.HandleCommand(context, command);
        }

        return Task.FromResult(new GameCommandResult());
    }

    public async Task<IReadOnlyList<GameCommand>> ProviderCommands(IWorldGrain world, IPassageGrain passage)
    {
        var commands = new List<GameCommand>();
        var context = new GameCommandContext(world, passage);
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
        return new GameCommandAction(ToKind(parts[0]), Int64.Parse(parts[^1]));
    }

    public GameCommandAction(string kind, long passageId)
    {
        Kind = kind;
        PassageId = passageId;
    }

    public string Kind { get; }
    public long PassageId { get; }

    public override string ToString()
    {
        return $"{FromKind(Kind)}:{PassageId}";
    }

    private static string FromKind(string commandKind)
    {
        return commandKind;
    }

    private static string ToKind(string actionTag)
    {
        return actionTag;
    }
}
