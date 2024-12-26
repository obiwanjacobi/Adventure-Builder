using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class CommandBar : ComponentBase
{
    [Parameter]
    public Func<string, Task>? ExecuteCommandAsync { get; set; }

    [Parameter]
    public IReadOnlyList<GameCommandInfo> CommandInfos { get; set; } = [];
}
