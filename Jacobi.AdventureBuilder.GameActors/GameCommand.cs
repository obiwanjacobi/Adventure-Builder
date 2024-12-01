using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class GameCommandHandler
{
    private readonly IWorldGrain world;
    private readonly IPlayerGrain player;

    public GameCommandHandler(IWorldGrain world, IPlayerGrain player)
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