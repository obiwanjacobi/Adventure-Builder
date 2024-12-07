using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class Home : ComponentBase
{
    private string? _name;
    private string? _description;
    private IReadOnlyList<GameCommandInfo>? _commands;
    private IReadOnlyList<GameExtraInfo>? _extras;
    private IPlayerGrain? _player;
    private IWorldGrain? _world;
    private IPassageGrain? _passage;
    private string? _worldName;
    private string? _playerNickname;

    protected override async Task OnInitializedAsync()
    {
        _world = await _gameClient.WorldManager.CreateNewWorld("CA59D7B3-C88B-499E-A333-9BC5D2906506", "My First Little Adventure World");
        _worldName = await _world.Name();
        _player = _gameClient.GetPlayer("PlayerOne");
        _playerNickname = PlayerKey.Parse(_player.GetPrimaryKeyString()).Nickname;
        var passage = await _world.Start(_player);
        await SetPassage(passage);
    }

    private async Task ExecuteCommand(string commandId)
    {
        var command = await _passage!.GetCommand(commandId);
        var result = await _player!.Play(_world!, command);
        if (result.Passage is null)
        {
            _name = "Error";
            _description = "There is a configuration error. This navigation option does not result in a Passage.";
            return;
        }

        await SetPassage(result.Passage);
    }

    private async Task SetPassage(IPassageGrain passage)
    {
        _passage = passage;
        _name = await _passage.Name();
        _description = await _passage.Description();
        _commands = await _passage.CommandInfos();
        _extras = await _passage.Extras();
    }

}
