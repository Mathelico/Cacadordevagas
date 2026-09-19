using System.Text.Json.Serialization;

namespace JobHunterAI.Api.Models;

public class JoobleJob
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("location")]
    public string Location { get; set; } = "";

    [JsonPropertyName("snippet")]
    public string Snippet { get; set; } = "";

    [JsonPropertyName("salary")]
    public string Salary { get; set; } = "";

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("link")]
    public string Link { get; set; } = "";

    [JsonPropertyName("company")]
    public string Company { get; set; } = "";

    [JsonPropertyName("updated")]
    public DateTime Updated { get; set; }
}

public class JoobleResponse
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("jobs")]
    public List<JoobleJob> Jobs { get; set; } = new();
}