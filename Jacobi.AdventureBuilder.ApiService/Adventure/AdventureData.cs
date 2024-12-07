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

    [JsonProperty("passages")]
    public List<AdventurePassageData> Passages { get; set; } = [];

    [JsonProperty("nonPlayerCharacters")]
    public List<AdventureNonPlayerCharacterData> NonPlayerCharacters { get; set; } = [];

    [JsonProperty("assets")]
    public List<AdventureAssetData> Assets { get; set; } = [];

    internal sealed record AdventurePassageData
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

    internal sealed record AdventureNonPlayerCharacterData
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = String.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = String.Empty;

        [JsonProperty("linkedPassages")]
        public List<long> LinkedPassageIds { get; set; } = [];
    }

    internal sealed record AdventureAssetData
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = String.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = String.Empty;

        [JsonProperty("linkedPassages")]
        public List<long> LinkedPassageIds { get; set; } = [];
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
