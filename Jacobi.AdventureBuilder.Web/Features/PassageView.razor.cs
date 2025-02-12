using Jacobi.AdventureBuilder.GameClient;
using Jacobi.AdventureBuilder.GameContracts;
using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features;

public partial class PassageView : ComponentBase
{
    private string? _name;
    private string? _description;
    private List<OccupantInfo> _npcs = [];
    private List<OccupantInfo> _assets = [];
    private List<OccupantInfo> _players = [];
    private readonly IGrainFactory _grainFactory;

    public PassageView(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [Parameter]
    public IPassageGrain? Passage { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await UpdateAsync();
    }

    public async Task UpdateAsync()
    {
        if (Passage is null) return;

        _players.Clear();
        _npcs.Clear();
        _assets.Clear();

        _name = await Passage.Name();
        _description = await Passage.Description();

        var occupantKeys = await Passage.Occupants();

        foreach (var occupantKey in occupantKeys)
        {
            var occupant = _grainFactory.GetPassageOccupant(occupantKey, out var tag);

            if (occupant is not null)
            {
                var info = new OccupantInfo(await occupant.Name(), await occupant.Description());

                switch (tag)
                {
                    case PlayerKey.Tag:
                        _players.Add(info);
                        break;
                    case NonPlayerCharacterKey.Tag:
                        _npcs.Add(info);
                        break;
                    case AssetKey.Tag:
                        _assets.Add(info);
                        break;
                    default:
                        throw new NotImplementedException(
                            $"PassageView has no implementation for '{tag}' type of IPassageOccupant object.");
                };
            };
        }

        StateHasChanged();
    }

    private record class OccupantInfo(string Name, string Description);
}
