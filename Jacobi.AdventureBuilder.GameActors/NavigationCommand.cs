using System.Diagnostics;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

internal sealed class NavigationCommandHandler : IGameCommandHandler
{
    public const string NavigatePassage = "nav-passage";

    public bool CanHandleCommand(GameCommand command)
    {
        return command.Kind == NavigatePassage;
    }

    public async Task<GameCommandResult> HandleCommand(GameCommandContext context, GameCommand command)
    {
        if (context.Issuer is null) throw new InvalidOperationException(
            $"The '{command.Action}' command requires the '{nameof(GameCommandContext.Issuer)}'.");

        var cmdAction = GameCommandAction.Parse(command.Action);
        var passageGrain = await context.World.GetPassage(cmdAction.Id);
        await context.Issuer.EnterPassage(context, passageGrain);
        return new GameCommandResult(passageGrain);
    }

    public async Task<IReadOnlyList<GameCommand>> ProvideCommands(GameCommandContext context)
    {
        Debug.Assert(context.Passage is not null);

        var links = await context.Passage.Links();
        var worldKey = WorldKey.Parse(context.World.GetPrimaryKeyString());

        var commands = links.Map(lnk =>
            new GameCommand(
                NavigatePassage,
                lnk.Name, lnk.Description,
                new GameCommandAction(NavigatePassage, lnk.PassageId).ToString(),
                $"Navigate to {lnk.Name}",
                new PassageKey(worldKey, lnk.PassageId)
            )
        ).ToList();

        return commands;
    }

    public static bool IsNavigationCommand(GameCommand command)
        => command.Kind == NavigatePassage;
}

