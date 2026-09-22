using JobHunterAI.Api.Services;
using Microsoft.AspNetCore.Mvc;
using JobHunterAI.Api.Data;
using JobHunterAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobHunterAI.Api.Controllers;

[ApiController]
[Route("api/vagas")]
public class VagasController : ControllerBase
{
    private readonly RemotiveService _remotiveService;
    private readonly JoobleService _joobleService;
    private readonly OpenAIService _openAIService;
    private readonly AppDbContext _context;
    private readonly VagaSearchService _vagaSearchService;

    public VagasController(
        OpenAIService openAIService,
        AppDbContext context,
        RemotiveService remotiveService,
        JoobleService joobleService,
        VagaSearchService vagaSearchService)
    {
        _openAIService = openAIService;
        _context = context;
        _remotiveService = remotiveService;
        _joobleService = joobleService;
        _vagaSearchService = vagaSearchService;
    }

    // Analisa manualmente uma vaga e salva no banco
    [HttpPost("analisar")]
    public async Task<IActionResult> Analisar(
        [FromBody] AnalisarVagaRequest request)
    {
        var resultado =
            await _openAIService.AnalisarVaga(request.Descricao);

        var vaga = new Vaga
        {
            Cargo = request.Cargo,
            Empresa = request.Empresa,
            Descricao = request.Descricao,
            Link = request.Link,

            Compatibilidade = resultado.Compatibilidade,
            Recomendacao = resultado.Recomendacao,
            NivelVaga = resultado.NivelVaga,
            Justificativa = resultado.Justificativa
        };

        _context.Vagas.Add(vaga);

        await _context.SaveChangesAsync();

        return Ok(vaga);
    }

    // Lista todas as vagas
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var vagas = await _context.Vagas
            .OrderByDescending(v => v.DataAnalise)
            .ToListAsync();

        return Ok(vagas);
    }

    // Busca uma vaga pelo ID
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var vaga = await _context.Vagas.FindAsync(id);

        if (vaga == null)
        {
            return NotFound(new
            {
                mensagem = "Vaga não encontrada."
            });
        }

        return Ok(vaga);
    }

    // Exclui uma vaga
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var vaga = await _context.Vagas.FindAsync(id);

        if (vaga == null)
        {
            return NotFound(new
            {
                mensagem = "Vaga não encontrada."
            });
        }

        _context.Vagas.Remove(vaga);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Vaga excluída com sucesso."
        });
    }

    // Busca vagas no Remotive
    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarVagas()
    {
        var vagas = await _remotiveService.BuscarVagas();

        return Ok(vagas);
    }

    // Busca vagas no Jooble
    [HttpGet("buscar-jooble")]
    public async Task<IActionResult> BuscarJooble()
    {
        var vagas = await _joobleService.BuscarVagas();

        return Ok(vagas);
    }

    // Busca vagas em todas as fontes
    [HttpGet("buscar-todas")]
    public async Task<IActionResult> BuscarTodas()
    {
        var vagas = await _vagaSearchService.BuscarTodas();

        return Ok(vagas);
    }

    // Analisa apenas a primeira vaga encontrada
    [HttpGet("analisar-primeira")]
    public async Task<IActionResult> AnalisarPrimeira()
    {
        var resultado =
            await _vagaSearchService.AnalisarPrimeiraVaga();

        if (resultado == null)
        {
            return NotFound(new
            {
                mensagem = "Nenhuma vaga encontrada."
            });
        }

        return Ok(resultado);
    }

    // Analisa várias vagas sem salvar
    [HttpGet("analisar-vagas")]
    public async Task<IActionResult> AnalisarVagas()
    {
        var resultado =
            await _vagaSearchService.AnalisarVagas();

        return Ok(resultado);
    }

    // Busca, analisa e salva novas vagas
    [HttpPost("analisar-e-salvar")]
    public async Task<IActionResult> AnalisarESalvar(
        [FromQuery] int limite = 3)
    {
        var quantidade =
            await _vagaSearchService.AnalisarESalvarVagas(limite);

        return Ok(new
        {
            mensagem = "Análise concluída.",
            vagasSalvas = quantidade
        });
    }

    // Lista vagas recomendadas para candidatura
    [HttpGet("candidatar")]
    public async Task<IActionResult> ListarParaCandidatar()
    {
        var vagas = await _context.Vagas
            .Where(v => v.Recomendacao == "CANDIDATAR")
            .OrderByDescending(v => v.Compatibilidade)
            .ToListAsync();

        return Ok(vagas);
    }

    // Lista vagas que precisam ser analisadas
    [HttpGet("analisar")]
    public async Task<IActionResult> ListarParaAnalisar()
    {
        var vagas = await _context.Vagas
            .Where(v => v.Recomendacao == "ANALISAR")
            .OrderByDescending(v => v.Compatibilidade)
            .ToListAsync();

        return Ok(vagas);
    }

    // Lista vagas recomendadas para ignorar
    [HttpGet("ignorar")]
    public async Task<IActionResult> ListarParaIgnorar()
    {
        var vagas = await _context.Vagas
            .Where(v => v.Recomendacao == "IGNORAR")
            .OrderByDescending(v => v.Compatibilidade)
            .ToListAsync();

        return Ok(vagas);
    }

    // Retorna dados resumidos para o dashboard
    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo()
    {
        var total = await _context.Vagas.CountAsync();

        var candidatar = await _context.Vagas
            .CountAsync(v => v.Recomendacao == "CANDIDATAR");

        var analisar = await _context.Vagas
            .CountAsync(v => v.Recomendacao == "ANALISAR");

        var ignorar = await _context.Vagas
            .CountAsync(v => v.Recomendacao == "IGNORAR");

        var mediaCompatibilidade = total > 0
            ? await _context.Vagas.AverageAsync(
                v => v.Compatibilidade
            )
            : 0;

        return Ok(new
        {
            total,
            candidatar,
            analisar,
            ignorar,
            mediaCompatibilidade =
                Math.Round(mediaCompatibilidade, 1)
        });
    }

    // Marca ou desmarca uma vaga como visualizada
    [HttpPatch("{id}/visualizada")]
    public async Task<IActionResult> AtualizarVisualizada(
        int id,
        [FromBody] bool visualizada)
    {
        var vaga = await _context.Vagas.FindAsync(id);

        if (vaga == null)
        {
            return NotFound(new
            {
                mensagem = "Vaga não encontrada."
            });
        }

        vaga.Visualizada = visualizada;

        await _context.SaveChangesAsync();

        return Ok(vaga);
    }
}

public class AnalisarVagaRequest
{
    public string Cargo { get; set; } = "";

    public string Empresa { get; set; } = "";

    public string Descricao { get; set; } = "";

    public string Link { get; set; } = "";
}