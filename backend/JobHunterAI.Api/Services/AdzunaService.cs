using System.Text.Json;
using JobHunterAI.Api.Models;
using System.Text;

namespace JobHunterAI.Api.Services;

public class AdzunaService
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _appKey;

    public AdzunaService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _appId = configuration["Adzuna:AppId"]
            ?? throw new Exception(
                "AppId da Adzuna não configurado."
            );

        _appKey = configuration["Adzuna:AppKey"]
            ?? throw new Exception(
                "AppKey da Adzuna não configurada."
            );
    }

    public async Task<List<AdzunaJob>> BuscarVagas()
    {
        var appId = Uri.EscapeDataString(_appId);
        var appKey = Uri.EscapeDataString(_appKey);

        var cargo = Uri.EscapeDataString(
            "desenvolvedor junior"
        );

        var localizacao = Uri.EscapeDataString(
            "Curitiba"
        );

        var url =
            "https://api.adzuna.com/v1/api/jobs/br/search/1" +
            $"?app_id={appId}" +
            $"&app_key={appKey}" +
            $"&results_per_page=20" +
            $"&what={cargo}" +
            $"&where={localizacao}" +
            $"&content-type=application/json";

        var response =
            await _httpClient.GetAsync(url);

        var responseBytes =
            await response.Content.ReadAsByteArrayAsync();

        var responseContent =
         Encoding.UTF8.GetString(responseBytes);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Erro da Adzuna: " +
                $"{response.StatusCode} - " +
                $"{responseContent}"
            );
        }

        var resultado =
            JsonSerializer.Deserialize<AdzunaResponse>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        var vagas =
            resultado?.Results ?? new List<AdzunaJob>();

        return vagas
            .Where(VagaCompativelComBusca)
            .ToList();
            }
            private bool VagaCompativelComBusca(AdzunaJob vaga)
        {
            var titulo = vaga.Title.ToLowerInvariant();
            var descricao = vaga.Description.ToLowerInvariant();

            var textoCompleto = $"{titulo} {descricao}";

            var termosDesejados = new[]
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
                "automação",
                "rpa",
                "suporte"
            };

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
                termosDesejados.Any(termo =>
                    titulo.Contains(termo));

            var possuiTermoIndesejado =
                termosIndesejados.Any(termo =>
                    textoCompleto.Contains(termo));

            return possuiAreaDesejada &&
                !possuiTermoIndesejado;
        }
}