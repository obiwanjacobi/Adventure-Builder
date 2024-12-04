using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public class NonPlayerCharacterGrainState
{
    public bool IsLoaded { get; set; }
    public AdventureNonPlayerCharacterInfo? NpcInfo { get; set; }
    public IPassageGrain? Passage { get; set; }
    public long ExtraId { get; set; }
}

public sealed class NonPlayerCharacterGrain : Grain<NonPlayerCharacterGrainState>, INonPlayerCharacterGrain
{
    private readonly IAdventureClient _client;

    public NonPlayerCharacterGrain(IAdventureClient client)
        => _client = client;

    public async override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!State.IsLoaded)
        {
            State.IsLoaded = true;

            var key = NonPlayerCharacterKey.Parse(this.GetPrimaryKeyString());
            State.NpcInfo = await _client.GetAdventureNonPlayerCharacter(key.WorldKey.WorldId, key.NpcId);
        }

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task EnterPassage(IPassageGrain passage)
    {
        ThrowIfNotLoaded();

        if (State.Passage is not null)
        {
            await State.Passage.RemoveExtraInfo(State.ExtraId);
        }

        State.ExtraId = await passage.AddExtraInfo(new AdventureExtraInfo
        {
            Name = State.NpcInfo!.Name,
            Description = State.NpcInfo!.Description,
            PassageId = 0
        });

        State.Passage = passage;
        await WriteStateAsync();
    }

    private void ThrowIfNotLoaded()
    {
        if (!State.IsLoaded)
            throw new InvalidOperationException("This AdventureWorld has not been loaded.");
    }
}
