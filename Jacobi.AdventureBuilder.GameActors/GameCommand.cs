using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class GameCommandHandler
{
    private readonly IWorldGrain _world;
    private readonly IPlayerGrain _player;

    public GameCommandHandler(IWorldGrain world, IPlayerGrain player)
    {
        _world = world;
        _player = player;
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
        var passageGrain = await _world.GetPassage(targetPassageId);
        await _player.EnterPassage(passageGrain);
        return new GameCommandResult(passageGrain);

        static long ParsePassageId(string navPassageAction)
        {
            var index = navPassageAction.LastIndexOf(':');
            return Int64.Parse(navPassageAction[(index + 1)..]);
        }
    }
}