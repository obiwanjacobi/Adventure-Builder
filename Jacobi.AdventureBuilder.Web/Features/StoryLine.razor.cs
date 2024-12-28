using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class StoryLine : ComponentBase
{
    [Parameter]
    public IReadOnlyList<PlayerLogLine> Items { get; set; } = [];

    public Icon GetIcon(PlayerLogLineKind kind)
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
