using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

internal sealed class NavigationCommandHandler : IGameCommandHandler
{
    public bool CanHandleCommand(GameCommand command)
    {
        return command.Kind == GameCommands.NavigatePassage;
    }

    public async Task<GameCommandResult> HandleCommand(GameCommandContext context, GameCommand command)
    {
        var cmdAction = GameCommandAction.Parse(command.Action);
        var passageGrain = await context.World.GetPassage(cmdAction.PassageId);
        await context.Issuer.EnterPassage(passageGrain);
        return new GameCommandResult(passageGrain);
    }
}
