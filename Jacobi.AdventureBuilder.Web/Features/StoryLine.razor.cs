using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class StoryLine : ComponentBase
{
    [Parameter]
    public IReadOnlyList<PlayerLogLine> Items { get; set; } = [];
}
