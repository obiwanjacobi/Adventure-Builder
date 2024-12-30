using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using Jacobi.AdventureBuilder.Web.Features.Notification;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class Home : ComponentBase
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly AdventureGameClient _gameClient;
    private NotificationClient _notificationClient;
    private string? _name;
    private string? _description;
    private IReadOnlyList<GameCommand>? _commands;
    private IPlayerGrain? _player;
    private IWorldGrain? _world;
    private IPassageGrain? _passage;
    private string? _worldName;
    private string? _playerNickname;
    private IReadOnlyList<PlayerLogLine> _logLines = [];

    public Home(AuthenticationStateProvider authenticationStateProvider, AdventureGameClient gameClient, NavigationManager navigationManager)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _gameClient = gameClient;
        _notificationClient = new NotificationClient(navigationManager);
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            _world = await _gameClient.WorldManager.CreateNewWorld("CA59D7B3-C88B-499E-A333-9BC5D2906506", "My First Little Adventure World");
            _worldName = await _world.Name();
            _player = _gameClient.GetPlayer(user.Identity.Name!);
            _playerNickname = PlayerKey.Parse(_player.GetPrimaryKeyString()).Nickname;
            var passage = await _world.Start(_player);
            await SetPassage(passage);

            _notificationClient.OnPassageEnter = OnPassageEnter;
            _notificationClient.OnPassageExit = OnPassageExit;
            await _notificationClient.StartAsync(_player.GetPrimaryKeyString(), passage.GetPrimaryKeyString());
        }
    }

    private async Task OnPassageExit(string characterId)
    {
        if (_player is null || _passage is null) return;
        var playerKey = _player.GetPrimaryKeyString();
        if (playerKey == characterId) return;

        var log = await _player.Log();
        await log.UpdateLine(_passage, playerKey);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnPassageEnter(string characterId)
    {
        if (_player is null || _passage is null) return;
        var playerKey = _player.GetPrimaryKeyString();
        if (playerKey == characterId) return;

        var log = await _player.Log();
        await log.UpdateLine(_passage, playerKey);
        await InvokeAsync(StateHasChanged);
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
        StateHasChanged();
    }

    private async Task SetPassage(IPassageGrain passage)
    {
        var log = await _player!.Log();

        _passage = passage;
        _name = await _passage.Name();
        _description = await _passage.Description();
        _commands = await _passage.Commands();
        _logLines = await log.Lines();
    }
}

