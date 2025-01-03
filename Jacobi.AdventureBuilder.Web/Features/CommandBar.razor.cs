using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class CommandBar : ComponentBase
{
    [Parameter]
    public Func<string, Task>? ExecuteCommandAsync { get; set; }

    [Parameter]
    public IReadOnlyList<GameCommand> Commands { get; set; } = [];

    public static Icon GetIcon(string cmdKind)
    {
        return cmdKind switch
        {
            "nav-passage" => new Icons.Regular.Size20.ArrowCurveUpRight(),
            "inv-put" => new Icons.Regular.Size20.AddSquare(),
            "inv-take" => new Icons.Regular.Size20.ArrowExit(),
            _ => new Icons.Regular.Size20.QuestionCircle(),
        };
    }
}
