using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class AdventureWorldState
{
    public string WorldId { get; set; } = String.Empty;
}

public sealed class AdventureWorld : Grain<AdventureWorldState>, IAdventureWorld
{
    public string Id
        => this.State.WorldId;

    public Task Start()
    {
        throw new NotImplementedException();
    }

    public Task Stop()
    {
        throw new NotImplementedException();
    }
}
