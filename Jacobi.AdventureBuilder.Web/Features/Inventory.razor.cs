using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class Inventory : ComponentBase
{
    [Parameter]
    public IReadOnlyList<InventoryItem> Items { get; set; } = [];
}

public sealed class InventoryItem
{
    public InventoryItem(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
}
