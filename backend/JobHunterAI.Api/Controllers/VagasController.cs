using JobHunterAI.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobHunterAI.Api.Controllers;

[ApiController]
[Route("api/vagas")]
public class VagasController : ControllerBase
{
    private readonly OpenAIService _openAIService;

    public VagasController(OpenAIService openAIService)
    {
        _openAIService = openAIService;
    }

    [HttpPost("analisar")]
    public async Task<IActionResult> Analisar(
        [FromBody] AnalisarVagaRequest request)
    {
        var resultado =
            await _openAIService.AnalisarVaga(request.Descricao);

        return Ok(new
        {
            analise = resultado
        });
    }
}

public class AnalisarVagaRequest
{
    public string Descricao { get; set; } = "";
}