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
            new StoryLineItem(line.Kind, line.Title, line.Description, line.CommandKind, cnt++ == 0,
                line.SubLines?.Select(subLine =>
                    new StoryLineItem(subLine.Kind, subLine.Title, subLine.Description, null, cnt++ == 0, [])
                ).ToList() ?? [])
        )
        .ToList();
    }

    private static Icon GetIcon(StoryLineItem item)
    {
        return item.Kind switch
        {
            PlayerLogLineKind.Asset => new Icons.Regular.Size20.Notebook(),
            PlayerLogLineKind.Command => GetCommandIcon(item),
            PlayerLogLineKind.NonPlayerCharacter => new Icons.Regular.Size20.Person(),
            PlayerLogLineKind.Passage => new Icons.Regular.Size20.Location(),
            PlayerLogLineKind.Player => new Icons.Regular.Size20.PersonHome(),
            _ => new Icons.Regular.Size20.Box(),
        };
    }

    private static Icon GetCommandIcon(StoryLineItem item)
    {
        return item.CommandKind switch
        {
            "nav-passage" => new Icons.Regular.Size20.ArrowCurveUpRight(),
            "inv-put" => new Icons.Regular.Size20.AddSquare(),
            "inv-take" => new Icons.Regular.Size20.ArrowExit(),
            _ => new Icons.Regular.Size20.KeyCommand(),
        };
    }
}

public sealed record class StoryLineItem(
    PlayerLogLineKind Kind, string Title, string Description, string? CommandKind, bool Expanded,
    List<StoryLineItem> SubItems);
