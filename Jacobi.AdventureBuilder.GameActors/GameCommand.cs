using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class GameCommandHandler
{
    private readonly IWorldGrain _world;
    private readonly IAmInPassage _issuer;
    private readonly Option<INotifyPassage> _notifyPassage;

    public GameCommandHandler(IWorldGrain world, IAmInPassage issuer, INotifyPassage notifyPassage)
    {
        _world = world;
        _issuer = issuer;
        _notifyPassage = Option<INotifyPassage>.Some(notifyPassage);
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
        await _issuer.EnterPassage(passageGrain, _notifyPassage);
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