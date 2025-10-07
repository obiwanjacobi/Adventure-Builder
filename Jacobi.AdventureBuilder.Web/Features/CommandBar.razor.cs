using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Icons.Regular;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class CommandBar : ComponentBase
{
    [Parameter]
    public required Func<string, Task> ExecuteCommandAsync { get; set; }

    [Parameter]
    public IReadOnlyList<GameCommand> Commands { get; set; } = [];

    public static Icon GetIcon(string cmdKind)
    {
        return cmdKind switch
        {
            "nav-passage" => new Size20.ArrowCurveUpRight(),
            "inv-put" => new Size20.AddSquare(),
            "inv-take" => new Size20.ArrowExit(),
            _ => new Size20.QuestionCircle(),
        };
    }
}
