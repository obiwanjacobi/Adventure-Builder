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
        var title = $"Navigate to {command.Name}";
        var description = command.Description;
        var line = new PlayerLogLine(id, PlayerLogLineKind.Command, title, description);

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
            passage.GetPrimaryKeyString(), subLines);

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
        var amInPassage = _factory.GetAmInPassage(grainKey, out var tag);
        PlayerLogLineKind kind = PlayerLogLineKind.None;

        amInPassage.IfSome(amInPassage =>
        {
            kind = tag switch
            {
                PlayerKey.Tag => PlayerLogLineKind.Player,
                NonPlayerCharacterKey.Tag => PlayerLogLineKind.NonPlayerCharacter,
                AssetKey.Tag => PlayerLogLineKind.Asset,
                _ => throw new NotImplementedException(
                    $"No implementation for '{tag}' type of IAmInPassage object.")
            };
        });

        if (kind != PlayerLogLineKind.None &&
            amInPassage.TryGetValue(out var amInPassageGrain))
        {
            return await NewLine(amInPassageGrain, kind, grainKey);
        }

        throw new NotSupportedException($"No {nameof(PlayerLogLine)} could be created for '{grainKey}'.");
    }

    private async Task<PlayerLogLine> NewLine(IAmInPassage amInPassage, PlayerLogLineKind kind, string grainKey)
    {
        var id = State.LogLines.Count;
        var title = await amInPassage.Name();
        var description = await amInPassage.Description();
        var line = new PlayerLogLine(id, kind, title, description, grainKey);
        return line;
    }
}
