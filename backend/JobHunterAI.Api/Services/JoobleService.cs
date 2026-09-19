using System.Text;
using System.Text.Json;
using JobHunterAI.Api.Models;

namespace JobHunterAI.Api.Services;

public class JoobleService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public JoobleService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Jooble:ApiKey"]
            ?? throw new Exception(
                "Chave da Jooble não configurada."
            );
    }

    public async Task<List<JoobleJob>> BuscarVagas()
    {
        var url =
            $"https://br.jooble.org/api/{_apiKey}";

        var body = new
        {
            keywords = "desenvolvedor junior",
            location = "Curitiba",
            radius = "40",
            page = 1,
            companysearch = false
        };

        var json = JsonSerializer.Serialize(body);

        using var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response =
            await _httpClient.PostAsync(url, content);

        var responseContent =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Erro da Jooble: {response.StatusCode} - {responseContent}"
            );
        }

        var resultado =
            JsonSerializer.Deserialize<JoobleResponse>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        var vagas = resultado?.Jobs ?? new List<JoobleJob>();

        var vagasFiltradas = vagas
            .Where(VagaCompativelComBusca)
            .ToList();

        return vagasFiltradas;
    }

private bool VagaCompativelComBusca(JoobleJob vaga)
    {
        var titulo = vaga.Title.ToLowerInvariant();
        var descricao = vaga.Snippet.ToLowerInvariant();

        var textoCompleto = $"{titulo} {descricao}";

        // Áreas que realmente queremos
        var termosDesejadosNoTitulo = new[]
        {
            "desenvolvedor",
            "developer",
            "software",
            ".net",
            "c#",
            "csharp",
            "react",
            "frontend",
            "front-end",
            "backend",
            "back-end",
            "full stack",
            "fullstack",
            "programador",
            "programming",
            "automação",
            "rpa",
            "suporte"
        };

        // Senioridades que não queremos
        var termosIndesejados = new[]
        {
            "senior",
            "sênior",
            "sr",
            "pleno",
            " pl ",
            "lead",
            "principal",
            "staff",
            "manager",
            "diretor",
            "head",
            "estágio",
            "estagiário",
            "intern",
            "internship"
        };

        var possuiAreaDesejada =
            termosDesejadosNoTitulo.Any(termo =>
                titulo.Contains(termo));

        var possuiTermoIndesejado =
            termosIndesejados.Any(termo =>
                textoCompleto.Contains(termo));

        return possuiAreaDesejada &&
            !possuiTermoIndesejado;
    }
}