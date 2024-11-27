using Jacobi.AdventureBuilder.ApiService.Data;
using Newtonsoft.Json;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed record AdventureWorldData : Entity, ILogicalPartition
{
    public AdventureWorldData(string id)
        : base(id)
    { }

    public static string ContainerName { get; } = nameof(AdventureWorldData);

    [JsonProperty("name")]
    public string Name { get; set; } = String.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("rooms")]
    public List<AdventureRoomData> Rooms { get; set; } = [];

    internal sealed record AdventureRoomData
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = String.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("commands")]
        public List<AdventureCommandData> Commands { get; set; } = [];
    }

    internal sealed record AdventureCommandData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = String.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; } = String.Empty;
    }
}
