using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JobHunterAI.Api.Models;

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

    public async Task<AnaliseVagaResponse> AnalisarVaga(
        string descricaoVaga)
    {
        var prompt = $$"""
            Você é um sistema especializado em recrutamento
            de profissionais de tecnologia.

            Compare o perfil do candidato com a vaga apresentada.

            PERFIL DO CANDIDATO:

            {{PerfilCandidato.Perfil}}


            VAGA:

            {{descricaoVaga}}


            Analise a compatibilidade considerando:
            - experiência profissional
            - formação
            - tecnologias
            - nível de senioridade
            - requisitos obrigatórios
            - requisitos desejáveis

            Não invente experiências ou conhecimentos
            que não estejam no perfil.

            Responda SOMENTE com um JSON válido.

            Não utilize Markdown.
            Não utilize blocos de código.
            Não escreva nenhum texto antes ou depois do JSON.

            Utilize exatamente esta estrutura:

            {
                "compatibilidade": 0,
                "recomendacao": "CANDIDATAR",
                "pontosFortes": [
                    "item"
                ],
                "conhecimentosAusentes": [
                    "item"
                ],
                "nivelVaga": "Júnior",
                "justificativa": "explicação curta"
            }
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

        using var document =
            JsonDocument.Parse(responseContent);

        var output =
            document.RootElement.GetProperty("output");

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty(
                    "content",
                    out var content))
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty(
                        "type",
                        out var type) &&
                    type.GetString() == "output_text" &&
                    contentItem.TryGetProperty(
                        "text",
                        out var text))
                {
                    var textoJson = text.GetString();

                    if (string.IsNullOrWhiteSpace(textoJson))
                    {
                        continue;
                    }

                    var analise =
                        JsonSerializer.Deserialize<AnaliseVagaResponse>(
                            textoJson,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }
                        );

                    if (analise != null)
                    {
                        analise.Recomendacao =
                        DefinirRecomendacao(analise.Compatibilidade);   

                        return analise;
                    }
                }
            }
        }

        throw new Exception(
            $"Não foi possível obter uma análise válida. Resposta: {responseContent}"
        );
    }

    private string DefinirRecomendacao(int compatibilidade)
{
    return compatibilidade switch
    {
        >= 80 => "CANDIDATAR",
        >= 60 => "ANALISAR",
        _ => "IGNORAR"
    };
}
}