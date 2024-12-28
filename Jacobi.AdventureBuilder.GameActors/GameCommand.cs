using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class GameCommandHandler
{
    private readonly IWorldGrain _world;
    private readonly IAmInPassage _issuer;

    public GameCommandHandler(IWorldGrain world, IAmInPassage issuer)
    {
        _world = world;
        _issuer = issuer;
    }

    public Task<GameCommandResult> ExecuteAsync(GameCommand command)
    {
        return command.Kind switch
        {
            GameCommands.NavigatePassage => ExecuteNavigatePassage(command),
            _ => Task.FromResult(new GameCommandResult()),
        };
    }

    private async Task<GameCommandResult> ExecuteNavigatePassage(GameCommand command)
    {
        var cmdAction = new CommandAction(command.Action);
        var passageGrain = await _world.GetPassage(cmdAction.PassageId);
        await _issuer.EnterPassage(passageGrain);
        return new GameCommandResult(passageGrain);
    }
}

public static class GameCommands
{
    public const string NavigatePassage = "nav-passage";
}

public readonly record struct CommandAction
{
    public CommandAction(string actionMoniker)
    {
        var parts = actionMoniker.Split(':');
        PassageId = Int64.Parse(parts[^1]);
    }

    public long PassageId { get; }
}
