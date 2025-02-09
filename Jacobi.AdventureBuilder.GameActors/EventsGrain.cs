using Orleans.Streams;

namespace Jacobi.AdventureBuilder.GameActors;

public abstract class EventsGrain : Grain
{
    public const string StreamProviderName = "AzureQueueProvider";

    public EventsGrain()
    {
        StreamProvider = this.GetStreamProvider(StreamProviderName);
    }

    protected IStreamProvider StreamProvider { get; }
}
