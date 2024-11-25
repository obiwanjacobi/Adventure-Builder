using System.Text.Json.Serialization;

namespace Jacobi.AdventureBuilder.Twine;

internal static class TwineModelConstants
{
    public const string Uninitialized = "<not set>";
}

internal sealed class TwineModel
{
    [JsonPropertyName("uuid")]
    public Guid Uuid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    [JsonPropertyName("creatorVersion")]
    public string? CreatorVersion { get; set; }

    [JsonPropertyName("schemaName")]
    public string? SchemaName { get; set; }

    [JsonPropertyName("schemaVersion")]
    public string? SchemaVersion { get; set; }

    [JsonPropertyName("createdAtMs")]
    public long CreatedAtMs { get; set; }

    [JsonPropertyName("passages")]
    public List<Passage> Passages { get; set; } = [];
}

internal sealed class Passage
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("tags")]
    public string Tags { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("id")]
    public string Id { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("text")]
    public string Text { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("links")]
    public List<Link> Links { get; set; } = [];

    [JsonPropertyName("hooks")]
    public List<Hook> Hooks { get; set; } = [];

    [JsonPropertyName("cleanText")]
    public string CleanText { get; set; } = TwineModelConstants.Uninitialized;
}

internal sealed class Link
{
    [JsonPropertyName("linkText")]
    public string LinkText { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("passageName")]
    public string PassageName { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("original")]
    public string Original { get; set; } = TwineModelConstants.Uninitialized;
}

internal sealed class Hook
{
    [JsonPropertyName("hookName")]
    public string HookName { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("hookText")]
    public string HookText { get; set; } = TwineModelConstants.Uninitialized;

    [JsonPropertyName("original")]
    public string Original { get; set; } = TwineModelConstants.Uninitialized;
}