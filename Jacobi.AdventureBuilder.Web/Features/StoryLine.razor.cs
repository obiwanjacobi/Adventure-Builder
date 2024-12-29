using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class StoryLine : ComponentBase
{
    private IReadOnlyList<PlayerLogLine> _logLines = [];
    [Parameter]
    public IReadOnlyList<PlayerLogLine> LogLines
    {
        get { return _logLines; }
        set
        {
            _logLines = value;
            SetStoryLineItems(value);
        }
    }

    public IReadOnlyList<StoryLineItem> Items { get; private set; } = [];

    private void SetStoryLineItems(IReadOnlyList<PlayerLogLine> logLines)
    {
        var cnt = 0;
        Items = logLines.Select(line =>
            new StoryLineItem(line.Kind, line.Title, line.Description, cnt++ == 0,
                line.SubLines?.Select(subLine => new StoryLineItem(subLine.Kind, subLine.Title, subLine.Description, cnt++ == 0, []))
                    .ToList() ?? [])
        )
        .ToList();
    }

    private static Icon GetIcon(PlayerLogLineKind kind)
    {
        return kind switch
        {
            PlayerLogLineKind.Asset => new Icons.Regular.Size20.Notebook(),
            PlayerLogLineKind.Command => new Icons.Regular.Size20.ArrowCurveUpRight(),
            PlayerLogLineKind.NonPlayerCharacter => new Icons.Regular.Size20.Person(),
            PlayerLogLineKind.Passage => new Icons.Regular.Size20.Location(),
            PlayerLogLineKind.Player => new Icons.Regular.Size20.PersonHome(),
            _ => new Icons.Regular.Size20.Box(),
        };
    }
}

public sealed record class StoryLineItem(
    PlayerLogLineKind Kind, string Title, string Description, bool Expanded,
    List<StoryLineItem> SubItems);
