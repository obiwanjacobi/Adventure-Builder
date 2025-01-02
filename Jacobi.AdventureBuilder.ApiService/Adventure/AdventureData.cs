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
        public List<AdventureLinkData> LinkedPassages { get; set; } = [];
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
        public List<AdventurePropertyData> Properties { get; set; } = [];
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
        public List<AdventurePropertyData> Properties { get; set; } = [];
    }

    internal sealed record AdventureLinkData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = String.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("passageId")]
        public long PassageId { get; set; }
    }

    internal sealed record AdventurePropertyData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = String.Empty;

        [JsonProperty("value")]
        public string? Value { get; set; }
    }
}
