using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerLogGrainState
{
    public bool IsLoaded { get; set; }
    public List<PlayerLogLine> LogLines { get; set; } = [];
}

public sealed class PlayerLogGrain(IGrainFactory factory) : Grain<PlayerLogGrainState>, IPlayerLogGrain
{
    private readonly IGrainFactory _factory = factory;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
        }

        return base.OnActivateAsync(cancellationToken);
    }

    public Task Clear()
    {
        State.LogLines.Clear();
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<PlayerLogLine>> Lines()
    {
        return Task.FromResult((IReadOnlyList<PlayerLogLine>)State.LogLines);
    }

    public async Task AddLine(GameCommand command)
    {
        var id = State.LogLines.Count;
        var title = command.DisplayName;
        var description = command.Description;
        var line = new PlayerLogLine(id, PlayerLogLineKind.Command, title, description, command.Kind);

        State.LogLines.Insert(0, line);
        await WriteStateAsync();
    }

    // playerKey is used to filter out the current player as occupant
    // this also prevents reentrant grain issues
    public async Task AddLine(IPassageGrain passage, string playerKey)
    {
        var subLines = new List<PlayerLogLine>();
        foreach (var occupant in await passage.Occupants())
        {
            if (playerKey != occupant)
                subLines.Add(await NewLine(occupant));
        }

        var id = State.LogLines.Count;
        var title = await passage.Name();
        var description = await passage.Description();
        var line = new PlayerLogLine(id, PlayerLogLineKind.Passage, title, description,
            null, subLines);

        State.LogLines.Insert(0, line);
        await WriteStateAsync();
    }

    public async Task UpdateLine(IPassageGrain passage, string playerKey)
    {
        // passage must be top line
        var topLine = State.LogLines[0];
        if (topLine.Kind != PlayerLogLineKind.Passage ||
            topLine.Title != await passage.Name())
        {
            return;
        }

        State.LogLines.RemoveAt(0);
        await AddLine(passage, playerKey);
        await WriteStateAsync();
    }

    private async Task<PlayerLogLine> NewLine(string grainKey)
    {
        var passageOccupant = _factory.GetPassageOccupant(grainKey, out var tag);
        PlayerLogLineKind kind = PlayerLogLineKind.None;

        passageOccupant.IfSome(passageOccupant =>
        {
            kind = tag switch
            {
                PlayerKey.Tag => PlayerLogLineKind.Player,
                NonPlayerCharacterKey.Tag => PlayerLogLineKind.NonPlayerCharacter,
                AssetKey.Tag => PlayerLogLineKind.Asset,
                _ => throw new NotImplementedException(
                    $"No implementation for '{tag}' type of IPassageOccupant object.")
            };
        });

        if (kind != PlayerLogLineKind.None &&
            passageOccupant.TryGetValue(out var occupant))
        {
            return await NewLine(occupant, kind);
        }

        throw new NotSupportedException($"No {nameof(PlayerLogLine)} could be created for '{grainKey}'.");
    }

    private async Task<PlayerLogLine> NewLine(IPassageOccupant passageOccupant, PlayerLogLineKind kind)
    {
        var id = State.LogLines.Count;
        var title = await passageOccupant.Name();
        var description = await passageOccupant.Description();
        var line = new PlayerLogLine(id, kind, title, description);
        return line;
    }
}
