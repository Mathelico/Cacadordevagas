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
    private readonly OpenAIService _openAIService;
    private readonly AppDbContext _context;

    public VagasController(
        OpenAIService openAIService,
        AppDbContext context)
    {
        _openAIService = openAIService;
        _context = context;
    }

    [HttpPost("analisar")]
    public async Task<IActionResult> Analisar(
        [FromBody] AnalisarVagaRequest request)
    {
        // Envia a descrição da vaga para a OpenAI
        var resultado =
            await _openAIService.AnalisarVaga(request.Descricao);

        // Cria a entidade que será salva no banco
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

        // Adiciona a vaga ao Entity Framework
        _context.Vagas.Add(vaga);

        // Executa o INSERT no PostgreSQL
        await _context.SaveChangesAsync();

        return Ok(vaga);
    }
    [HttpGet]
    public async Task<IActionResult> Listar()
        {
            var vagas = await _context.Vagas
                .OrderByDescending(v => v.DataAnalise)
                .ToListAsync();

            return Ok(vagas);
        }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
        {
        Console.WriteLine($"ENTROU NO BuscarPorId - ID: {id}");

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
}

    public class AnalisarVagaRequest
    {
        public string Cargo { get; set; } = "";

        public string Empresa { get; set; } = "";

        public string Descricao { get; set; } = "";

        public string Link { get; set; } = "";
    }