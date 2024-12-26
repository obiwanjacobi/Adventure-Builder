using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class StoryLine : ComponentBase
{
    [Parameter]
    public List<StoryLineItem> Items { get; set; } = [];
}

public sealed record class StoryLineItem(string Name, string Description);
