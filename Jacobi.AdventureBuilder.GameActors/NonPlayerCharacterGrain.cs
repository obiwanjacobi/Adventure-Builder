using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

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

    public NonPlayerCharacterGrain(IGrainFactory factory, IAdventureClient client)
    {
        _factory = factory;
        _client = client;
    }

    protected override string Name
        => State.NpcInfo!.Name;
    protected override string Description
        => State.NpcInfo!.Description;

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
            var timeSpan = TimeSpan.FromSeconds(val);
            _navigationTimer = this.RegisterGrainTimer(NavigateRandom, timeSpan, timeSpan);
        }
    }

    private async Task NavigateRandom(CancellationToken ct)
    {
        if (!State.Passage.TryGetValue(out var passage)) return;

        var commands = await passage.CommandInfos();
        var navCommands = commands.Where(cmd => cmd.Kind == GameCommands.NavigatePassage).ToArray();
        if (navCommands.Length == 0) return;

        var index = Random.Shared.Next(navCommands.Length);
        var cmd = await passage.GetCommand(navCommands[index].Id);

        var key = AssetKey.Parse(this.GetPrimaryKeyString());
        var world = _factory.GetGrain<IWorldGrain>(key.WorldKey);
        var cmdHandler = new GameCommandHandler(world, this);
        var result = await cmdHandler.ExecuteAsync(cmd);
    }
}
