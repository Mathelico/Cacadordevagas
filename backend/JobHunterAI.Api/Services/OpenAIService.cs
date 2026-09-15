using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace JobHunterAI.Api.Services;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new Exception(
                "Chave da OpenAI não configurada."
            );
    }

    public async Task<string> AnalisarVaga(string descricaoVaga)
    {
        var prompt = $"""
            Analise a seguinte vaga de emprego:

            {descricaoVaga}

            Informe:
            - Principais tecnologias exigidas
            - Nível da vaga
            - Principais requisitos
            - Um pequeno resumo da vaga
            """;

        var body = new
        {
            model = "gpt-5.6-luna",
            input = prompt
        };

        var json = JsonSerializer.Serialize(body);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/responses"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _apiKey
            );

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response =
            await _httpClient.SendAsync(request);

        var responseContent =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Erro da OpenAI: {response.StatusCode} - {responseContent}"
            );
        }

        using var document = JsonDocument.Parse(responseContent);

var output = document.RootElement.GetProperty("output");

foreach (var item in output.EnumerateArray())
{
    if (!item.TryGetProperty("content", out var content))
        continue;

    foreach (var contentItem in content.EnumerateArray())
    {
        if (contentItem.TryGetProperty("type", out var type) &&
            type.GetString() == "output_text" &&
            contentItem.TryGetProperty("text", out var text))
        {
            return text.GetString() ?? "";
        }
    }
}

    throw new Exception(
        $"A OpenAI não retornou texto na resposta. Resposta recebida: {responseContent}"
        );
    }
}