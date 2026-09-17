using System.Text.Json;
using JobHunterAI.Api.Models;

namespace JobHunterAI.Api.Services;

public class RemotiveService
{
    private readonly HttpClient _httpClient;

    public RemotiveService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

 public async Task<List<RemotiveJob>> BuscarVagas()
{
    Console.WriteLine(">>> REMOTIVE SERVICE NOVO EXECUTADO <<<");

    var url =
        "https://remotive.com/api/remote-jobs?search=junior";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Erro ao buscar vagas na Remotive: {response.StatusCode}"
            );
        }

        var json = await response.Content.ReadAsStringAsync();

        var resultado = JsonSerializer.Deserialize<RemotiveResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        var vagas = resultado?.Jobs ?? new List<RemotiveJob>();

        var vagasFiltradas = vagas
            .Where(VagaCompativelComBusca)
            .ToList();

            Console.WriteLine($"Remotive retornou: {vagas.Count}");
            Console.WriteLine($"Depois do nosso filtro: {vagasFiltradas.Count}");

            return vagasFiltradas;
    }
private bool VagaCompativelComBusca(RemotiveJob vaga)
{
    var titulo = vaga.Title.ToLowerInvariant();
    var localizacao = vaga.CandidateRequiredLocation.ToLowerInvariant();

    // Cargos que não queremos
    var termosIndesejados = new[]
    {
        "senior",
        "sr.",
        "sr ",
        "lead",
        "principal",
        "staff",
        "manager",
        "director",
        "head of",
        "intern",
        "internship"
    };

    if (termosIndesejados.Any(termo => titulo.Contains(termo)))
        return false;

    // Senioridades que nos interessam
    var senioridadesDesejadas = new[]
    {
        "junior",
        "jr",
        "jr.",
        "entry level",
        "entry-level",
        "graduate",
        "trainee"
    };

    // Áreas relacionadas a desenvolvimento
    var areasDesejadas = new[]
    {
        "developer",
        "engineer",
        "programmer",
        "programming",
        "development",
        "software",
        "frontend",
        "front-end",
        "backend",
        "back-end",
        "fullstack",
        "full-stack",
        ".net",
        "c#",
        "csharp",
        "react"
    };

    var possuiSenioridadeDesejada =
        senioridadesDesejadas.Any(termo => titulo.Contains(termo));

    var possuiAreaDesejada =
        areasDesejadas.Any(termo => titulo.Contains(termo));

    // Localizações que podemos considerar
    var localizacaoPermitida =
        localizacao.Contains("brazil") ||
        localizacao.Contains("brasil") ||
        localizacao.Contains("worldwide") ||
        localizacao.Contains("anywhere") ||
        string.IsNullOrWhiteSpace(localizacao);

    return possuiSenioridadeDesejada &&
           possuiAreaDesejada &&
           localizacaoPermitida;
}
}