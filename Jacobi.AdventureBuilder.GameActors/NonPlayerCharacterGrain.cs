using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public class NonPlayerCharacterGrainState : AmInPassageGrainState
{
    public bool IsLoaded { get; set; }
    public AdventureNonPlayerCharacterInfo? NpcInfo { get; set; }
}

public sealed class NonPlayerCharacterGrain : AmInPassageGrain<NonPlayerCharacterGrainState>, INonPlayerCharacterGrain
{
    private IDisposable? _navigationTimer;
    private readonly IGrainFactory _factory;
    private readonly IAdventureClient _client;
    private readonly GameCommandExecuter _commandExecuter;

    public NonPlayerCharacterGrain(IGrainFactory factory, IAdventureClient client, GameCommandExecuter commandExecuter)
    {
        _factory = factory;
        _client = client;
        _commandExecuter = commandExecuter;
    }

    public override Task<string> Name()
        => Task.FromResult(State.NpcInfo!.Name);
    public override Task<string> Description()
        => Task.FromResult(State.NpcInfo!.Description);

    public async override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = NonPlayerCharacterKey.Parse(this.GetPrimaryKeyString());
            State.NpcInfo = await _client.GetAdventureNonPlayerCharacter(key.WorldKey.WorldId, key.NpcId, cancellationToken);

            StartPeriodicTimerIfAny();
        }

        await base.OnActivateAsync(cancellationToken);
    }

    private void StartPeriodicTimerIfAny()
    {
        if (State?.NpcInfo is null) return;

        var periodTimer = State.NpcInfo.Properties.SingleOrDefault(p => p.Name == "NavEvery");
        if (periodTimer is AdventurePropertyInfo navProp
            && Int32.TryParse(navProp.Value, out var val))
        {
            if (_navigationTimer is not null)
            {
                _navigationTimer.Dispose();
                _navigationTimer = null;
            }

            var timeSpan = TimeSpan.FromSeconds(val);
            _navigationTimer = this.RegisterGrainTimer(NavigateRandom, timeSpan, timeSpan);
        }
    }

    private async Task NavigateRandom(CancellationToken ct)
    {
        if (State.Passage is null) return;

        var commands = await State.Passage.Commands();
        var navCommands = commands.Where(cmd => NavigationCommandHandler.IsNavigationCommand(cmd)).ToArray();
        if (navCommands.Length == 0) return;

        var index = Random.Shared.Next(navCommands.Length);
        var cmd = navCommands[index];

        var key = NonPlayerCharacterKey.Parse(this.GetPrimaryKeyString());
        var world = _factory.GetGrain<IWorldGrain>(key.WorldKey);
        var result = await _commandExecuter.ExecuteAsync(world, this, State.Passage!, cmd);
    }
}
