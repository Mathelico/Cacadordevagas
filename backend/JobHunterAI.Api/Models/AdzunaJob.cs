using System.Text.Json.Serialization;

namespace JobHunterAI.Api.Models;

public class AdzunaJob
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("redirect_url")]
    public string RedirectUrl { get; set; } = "";

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("company")]
    public AdzunaCompany Company { get; set; } = new();

    [JsonPropertyName("location")]
    public AdzunaLocation Location { get; set; } = new();
}

public class AdzunaCompany
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = "";
}

public class AdzunaLocation
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = "";
}

public class AdzunaResponse
{
    [JsonPropertyName("results")]
    public List<AdzunaJob> Results { get; set; } = new();
}