using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class PlayerLogGrainState
{
    public bool IsLoaded { get; set; }
    public List<PlayerLogLine> LogLines { get; set; } = [];
}

public sealed class PlayerLogGrain : Grain<PlayerLogGrainState>, IPlayerLogGrain
{
    private const int MaxLogLines = 5;

    public PlayerLogGrain(INotifyPassage notifyClient)
    {
        _notifyClient = notifyClient;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;
        }

        var timeSpan = TimeSpan.FromMilliseconds(1000);
        this.RegisterGrainTimer(OnProcessLogEvents, timeSpan, timeSpan);

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
        var line = await CreateLine(command);

        State.LogLines.Insert(0, line);
        await WriteStateAsync();
    }

    public async Task AddLine(IPassageGrain passage, GameCommand? command = null)
    {
        var subLines = new List<PlayerLogLine>();

        if (command is not null)
            subLines.Add(await CreateLine(command));

        var occupantLines = await CreateSubLines(await passage.Occupants());
        subLines.AddRange(occupantLines);

        var line = await CreateLine(passage, subLines);
        State.LogLines.Insert(0, line);

        while (State.LogLines.Count > MaxLogLines)
            State.LogLines.RemoveAt(State.LogLines.Count - 1);

        await WriteStateAsync();
    }

    public Task UpdateLine(IPassageGrain passage, GameCommand? command = null)
    {
        QueueLogEvent(passage.GetPrimaryKeyString(), command);
        return Task.CompletedTask;
    }

    private async Task UpdateLineInternal(IPassageGrain passage, GameCommand? command = null)
    {
        // passage must be top line
        var topLine = State.LogLines[0];
        if (topLine.Kind != PlayerLogLineKind.Passage ||
            topLine.Title != await passage.Name())
        {
            return;
        }

        var subLines = topLine.SubLines?.Where(sl =>
            {
                return !String.IsNullOrEmpty(sl.CommandKind) &&
                command?.Subject != sl.Subject;
            }).ToList() ?? [];

        if (command is not null)
            subLines.Add(await CreateLine(command));

        var occupantLines = await CreateSubLines(await passage.Occupants());
        subLines.AddRange(occupantLines);

        var line = await CreateLine(passage, subLines);
        State.LogLines.RemoveAt(0);
        State.LogLines.Insert(0, line);
        await WriteStateAsync();
    }

    private async Task<PlayerLogLine> CreateLine(string grainKey)
    {
        var passageOccupant = GrainFactory.GetPassageOccupant(grainKey, out var tag);
        PlayerLogLineKind kind = PlayerLogLineKind.None;

        if (tag is not null)
        {
            kind = tag switch
            {
                PlayerKey.Tag => PlayerLogLineKind.Player,
                NonPlayerCharacterKey.Tag => PlayerLogLineKind.NonPlayerCharacter,
                AssetKey.Tag => PlayerLogLineKind.Asset,
                _ => throw new NotImplementedException(
                    $"No implementation for '{tag}' type of IPassageOccupant object.")
            };
        };

        if (kind != PlayerLogLineKind.None &&
            passageOccupant.TryGetValue(out var occupant))
        {
            return await CreateLine(occupant, grainKey, kind);
        }

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

    private async Task<List<PlayerLogLine>> CreateSubLines(IReadOnlyList<string> occupants)
    {
        var playerKey = this.GetPrimaryKeyString();
        var subLines = new List<PlayerLogLine>();
        foreach (var occupant in occupants)
        {
            if (playerKey != occupant)
                subLines.Add(await CreateLine(occupant));
        }

        return subLines;
    }


    private readonly record struct DeferedLogEvent(string PassageKey, GameCommand? Command);
    private readonly List<DeferedLogEvent> _logEvents = [];
    private readonly INotifyPassage _notifyClient;

    private void QueueLogEvent(string passageKey, GameCommand? command)
    {
        // no use in queuing the same passage multiple times
        if (!_logEvents.Exists(le => le.PassageKey == passageKey))
            _logEvents.Add(new DeferedLogEvent(passageKey, command));
    }

    private async Task OnProcessLogEvents()
    {
        var passageKeys = new List<string>();

        while (_logEvents.Count > 0)
        {
            var logEvent = _logEvents[0];
            _logEvents.RemoveAt(0);
            await OnLogEvent(logEvent);

            if (!passageKeys.Contains(logEvent.PassageKey))
                passageKeys.Add(logEvent.PassageKey);
        }

        foreach (var passageKey in passageKeys)
        {
            var player = GrainFactory.GetPlayer(this.GetPrimaryKeyString());
            var passageKeyObj = PassageKey.Parse(passageKey);
            var passage = GrainFactory.GetPassage(passageKey);
            var world = GrainFactory.GetWorld(passageKeyObj.WorldKey);
            var context = new GameContext(world, player, passage);
            // TODO: this should be a player-log-changed notification
            await _notifyClient.NotifyPassageEnter(context, passageKey, "");
        }
    }

    private async Task OnLogEvent(DeferedLogEvent logEvent)
    {
        var passage = GrainFactory.GetGrain<IPassageGrain>(logEvent.PassageKey);
        await UpdateLineInternal(passage, logEvent.Command);
    }
}
