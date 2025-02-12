using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerLogGrainState
{
    public bool IsLoaded { get; set; }
    public List<PlayerLogLine> LogLines { get; set; } = [];
}

public sealed class PlayerLogGrain : Grain<PlayerLogGrainState>, IPlayerLogGrain
{
    private IPlayerEventsGrain? _playerEvents;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
        }

        _playerEvents = GrainFactory.GetPlayerEvents(this.GetPrimaryKeyString());

        return base.OnActivateAsync(cancellationToken);
    }

    public Task Clear()
    {
        State.LogLines.Clear();
        return NotifyPlayerLogChanged();
    }

    public Task<IReadOnlyList<PlayerLogLine>> Lines(int count = 5)
        => Task.FromResult((IReadOnlyList<PlayerLogLine>)State.LogLines.Take(count).ToList());

    public async Task AddLine(GameCommand command)
    {
        var line = await CreateLine(command);

        State.LogLines.Insert(0, line);
        await WriteStateAsync();
        await NotifyPlayerLogChanged();
    }

    // playerKey is used to filter out the current player as occupant
    // this also prevents reentrant grain issues
    public async Task AddLine(IPassageGrain passage, string playerKey, GameCommand? command = null)
    {
        var subLines = new List<PlayerLogLine>();

        if (command is not null)
            subLines.Add(await CreateLine(command));

        var occupantLines = await CreateSubLines(await passage.Occupants(), playerKey);
        subLines.AddRange(occupantLines);

        var line = await CreateLine(passage, subLines);
        State.LogLines.Insert(0, line);
        await WriteStateAsync();
        await NotifyPlayerLogChanged();
    }

    private async Task<PlayerLogLine> CreateLine(string grainKey)
    {
        var occupant = GrainFactory.GetPassageOccupant(grainKey, out var tag);
        var kind = PlayerLogLineKind.None;

        if (occupant is not null)
        {
            kind = tag switch
            {
                PlayerKey.Tag => PlayerLogLineKind.Player,
                NonPlayerCharacterKey.Tag => PlayerLogLineKind.NonPlayerCharacter,
                AssetKey.Tag => PlayerLogLineKind.Asset,
                _ => throw new NotImplementedException(
                    $"No implementation for '{tag}' type of IPassageOccupant object.")
            };

            return await CreateLine(occupant, grainKey, kind);
        };

        throw new NotSupportedException(
            $"No {nameof(PlayerLogLine)} could be created for '{grainKey}'.");
    }

    private async Task<PlayerLogLine> CreateLine(IPassageGrain passage, List<PlayerLogLine> subLines)
    {
        var id = State.LogLines.Count;
        var title = await passage.Name();
        var description = await passage.Description();
        var line = new PlayerLogLine(id, PlayerLogLineKind.Passage, title, description,
            null, passage.GetPrimaryKeyString(), subLines);
        return line;
    }

    private async Task<PlayerLogLine> CreateLine(IPassageOccupantGrain passageOccupant, string occupantKey, PlayerLogLineKind kind)
    {
        var id = State.LogLines.Count;
        var title = await passageOccupant.Name();
        var description = await passageOccupant.Description();
        var line = new PlayerLogLine(id, kind, title, description, null, occupantKey);
        return line;
    }

    private Task<PlayerLogLine> CreateLine(GameCommand command)
    {
        var id = State.LogLines.Count;
        var title = command.DisplayName;
        var description = command.Description;
        var line = new PlayerLogLine(id, PlayerLogLineKind.Command, title, description, command.Kind, command.Subject);
        return Task.FromResult(line);
    }

    private async Task<List<PlayerLogLine>> CreateSubLines(IReadOnlyList<string> occupants, string playerKey)
    {
        var subLines = new List<PlayerLogLine>();
        foreach (var occupant in occupants)
        {
            if (playerKey != occupant)
                subLines.Add(await CreateLine(occupant));
        }

        return subLines;
    }

    private Task NotifyPlayerLogChanged()
        => _playerEvents!.NotifyPlayerLogChanged(this.GetPrimaryKeyString());
}
