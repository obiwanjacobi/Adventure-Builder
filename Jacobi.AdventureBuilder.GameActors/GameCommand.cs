using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public interface IGameCommandHandler
{
    bool CanHandleCommand(GameCommand command);
    Task<GameCommandResult> HandleCommand(GameCommandContext context, GameCommand command);
}

public interface IGameCommandProvider
{
    Task<IReadOnlyList<GameCommandResult>> ProvideCommands();
}

public sealed record class GameCommandContext(
    IAmInPassage Issuer, IPassageGrain Passage, IWorldGrain World);

public sealed class GameCommandExecuter
{
    private readonly List<IGameCommandHandler> _handlers;

    public GameCommandExecuter(IEnumerable<IGameCommandHandler> handlers)
    {
        _handlers = handlers.ToList();
    }

    public Task<GameCommandResult> ExecuteAsync(IWorldGrain world, IAmInPassage issuer, IPassageGrain passage, GameCommand command)
    {
        var context = new GameCommandContext(issuer, passage, world);
        foreach (var handler in _handlers)
        {
            if (handler.CanHandleCommand(command))
                return handler.HandleCommand(context, command);
        }

        return Task.FromResult(new GameCommandResult());
    }
}

public static class GameCommands
{
    public const string NavigatePassage = "nav-passage";
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
