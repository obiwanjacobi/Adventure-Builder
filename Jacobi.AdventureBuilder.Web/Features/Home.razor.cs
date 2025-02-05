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
    private IReadOnlyList<GameCommand>? _commands;
    private IPlayerGrain? _player;
    private IWorldGrain? _world;
    private IPassageGrain? _passage;
    private string? _worldName;
    private string? _playerNickname;
    private IReadOnlyList<PlayerLogLine> _logLines = [];
    private IReadOnlyList<InventoryItem> _inventoryItems = [];

    public Home(AuthenticationStateProvider authenticationStateProvider, AdventureGameClient gameClient, NavigationManager navigationManager)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _gameClient = gameClient;
        _notificationClient = new NotificationClient(navigationManager);
    }

    protected override async Task OnInitializedAsync()
    {
        // pre-render and render will each call this
        if (!RendererInfo.IsInteractive) return;

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

    private async Task OnPassageExit(string occupantKey)
    {
        if (_player is null || _passage is null) return;
        var playerKey = _player.GetPrimaryKeyString();
        if (playerKey == occupantKey) return;

        await SetPassage(_passage);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnPassageEnter(string occupantKey)
    {
        if (_player is null || _passage is null) return;
        var playerKey = _player.GetPrimaryKeyString();
        if (playerKey == occupantKey) return;

        await SetPassage(_passage);
        await InvokeAsync(StateHasChanged);
    }

    private async Task ExecuteCommand(string commandAction)
    {
        var commands = await _passage!.Commands(_player!);
        // command may have disappeared
        var command = commands.SingleOrDefault(cmd => cmd.Action == commandAction);
        if (command is not null)
        {
            var result = await _player!.Play(_world!, _passage, command);
            if (result.Passage is null) return;
            await SetPassage(result.Passage);
        }
        else
        {
            await SetPassage(_passage);
        }

        StateHasChanged();
    }

    private async Task SetPassage(IPassageGrain passage)
    {
        _passage = passage;
        _commands = await _passage.Commands(_player);

        var log = _gameClient.GrainFactory.GetPlayerLog(_player!);
        _logLines = await log.Lines();

        var inventory = _gameClient.GrainFactory.GetPlayerInventory(_player!);
        var assets = await inventory.Assets();
        var inventoryItems = new List<InventoryItem>();
        foreach (var asset in assets)
        {
            inventoryItems.Add(new InventoryItem(
                asset.GetPrimaryKeyString(), await asset.Name(), await asset.Description()
            ));
        }
        _inventoryItems = inventoryItems;
    }
}
