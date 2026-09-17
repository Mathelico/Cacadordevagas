using System.Text.Json.Serialization;

namespace JobHunterAI.Api.Models;

public class RemotiveJob
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("job_type")]
    public string JobType { get; set; } = "";

    [JsonPropertyName("candidate_required_location")]
    public string CandidateRequiredLocation { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("publication_date")]
    public DateTime PublicationDate { get; set; }
}

public class RemotiveResponse
{
    [JsonPropertyName("job-count")]
    public int JobCount { get; set; }

    [JsonPropertyName("jobs")]
    public List<RemotiveJob> Jobs { get; set; } = new();
}