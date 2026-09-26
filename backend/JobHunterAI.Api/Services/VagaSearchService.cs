using System.Net;
using System.Text.RegularExpressions;
using JobHunterAI.Api.Models;
using JobHunterAI.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace JobHunterAI.Api.Services;

public class VagaSearchService
{
    private readonly RemotiveService _remotiveService;
    private readonly JoobleService _joobleService;
    private readonly OpenAIService _openAIService;
    private readonly AppDbContext _context;
    private readonly AdzunaService _adzunaService;

    public VagaSearchService(
        RemotiveService remotiveService,
        JoobleService joobleService,
        AdzunaService adzunaService,
        OpenAIService openAIService,
        AppDbContext context)
    {
        _remotiveService = remotiveService;
        _joobleService = joobleService;
        _adzunaService = adzunaService;
        _openAIService = openAIService;
        _context = context;
    }
        public async Task<List<VagaEncontrada>> BuscarTodas()
    {
        var vagasEncontradas = new List<VagaEncontrada>();

        var vagasRemotive =
            await _remotiveService.BuscarVagas();

        var vagasJooble =
            await _joobleService.BuscarVagas();

        var vagasAdzuna =
            await _adzunaService.BuscarVagas();

        // Remotive
        foreach (var vaga in vagasRemotive)
        {
            vagasEncontradas.Add(new VagaEncontrada
            {
                IdExterno = vaga.Id.ToString(),
                Titulo = vaga.Title,
                Empresa = vaga.CompanyName,
                Localizacao = vaga.CandidateRequiredLocation,
                Descricao = LimparHtml(vaga.Description),
                Link = vaga.Url,
                Fonte = "Remotive",
                DataPublicacao = vaga.PublicationDate
            });
        }

        // Jooble
        foreach (var vaga in vagasJooble)
        {
            vagasEncontradas.Add(new VagaEncontrada
            {
                IdExterno = vaga.Id.ToString(),
                Titulo = vaga.Title,
                Empresa = vaga.Company,
                Localizacao = vaga.Location,
                Descricao = LimparHtml(vaga.Snippet),
                Link = vaga.Link,
                Fonte = "Jooble",
                DataPublicacao = vaga.Updated
            });
        }

        // Adzuna
        foreach (var vaga in vagasAdzuna)
        {
            vagasEncontradas.Add(new VagaEncontrada
            {
                IdExterno = vaga.Id,
                Titulo = vaga.Title,
                Empresa = vaga.Company.DisplayName,
                Localizacao = vaga.Location.DisplayName,
                Descricao = LimparHtml(vaga.Description),
                Link = vaga.RedirectUrl,
                Fonte = "Adzuna",
                DataPublicacao = vaga.Created
            });
        }

        // Remove vagas duplicadas entre as fontes
        var vagasSemDuplicidade = vagasEncontradas
            .GroupBy(vaga => new
            {
                Titulo = NormalizarTexto(vaga.Titulo),
                Empresa = NormalizarTexto(vaga.Empresa)
            })
            .Select(grupo => grupo.First())
            .ToList();

        return vagasSemDuplicidade;
    }

    public async Task<VagaAnalisada?> AnalisarPrimeiraVaga()
    {
        var vagas = await BuscarTodas();

        var vaga = vagas.FirstOrDefault();

        if (vaga == null)
        {
            return null;
        }

        var descricaoParaAnalise = $"""
            Cargo: {vaga.Titulo}

            Empresa: {vaga.Empresa}

            Localização: {vaga.Localizacao}

            Descrição:
            {vaga.Descricao}
            """;

        var analise =
            await _openAIService.AnalisarVaga(descricaoParaAnalise);

        return new VagaAnalisada
        {
            Vaga = vaga,
            Analise = analise
        };
    }

    public async Task<List<VagaAnalisada>> AnalisarVagas(int limite = 3)
    {
        var vagas = await BuscarTodas();

        var vagasParaAnalisar = vagas
            .Take(limite)
            .ToList();

        var vagasAnalisadas = new List<VagaAnalisada>();

        foreach (var vaga in vagasParaAnalisar)
        {
            var descricaoParaAnalise = $"""
                Cargo: {vaga.Titulo}

                Empresa: {vaga.Empresa}

                Localização: {vaga.Localizacao}

                Descrição:
                {vaga.Descricao}
                """;

            var analise =
                await _openAIService.AnalisarVaga(descricaoParaAnalise);

            vagasAnalisadas.Add(new VagaAnalisada
            {
                Vaga = vaga,
                Analise = analise
            });
        }

        return vagasAnalisadas
            .OrderByDescending(v => v.Analise.Compatibilidade)
            .ToList();
    }

        public async Task<int> AnalisarESalvarVagas(int limite = 3)
    {
        // Busca todas as vagas disponíveis
        var vagasEncontradas = await BuscarTodas();

        // Busca no banco os links que já foram analisados
        var linksExistentes = await _context.Vagas
            .Select(v => v.Link)
            .ToListAsync();

        var linksExistentesSet = linksExistentes
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Mantém somente vagas que ainda não estão no banco
        var vagasNovas = vagasEncontradas
            .Where(vaga =>
                !linksExistentesSet.Contains(vaga.Link))
            .Take(limite)
            .ToList();

        var quantidadeSalva = 0;

        foreach (var vagaEncontrada in vagasNovas)
        {
            var descricaoParaAnalise = $"""
                Cargo: {vagaEncontrada.Titulo}

                Empresa: {vagaEncontrada.Empresa}

                Localização: {vagaEncontrada.Localizacao}

                Descrição:
                {vagaEncontrada.Descricao}
                """;

            // Somente vagas novas são enviadas para a OpenAI
            var analise =
                await _openAIService.AnalisarVaga(
                    descricaoParaAnalise
                );

            var vaga = new Vaga
            {
                Cargo = vagaEncontrada.Titulo,
                Empresa = vagaEncontrada.Empresa,
                Descricao = vagaEncontrada.Descricao,
                Link = vagaEncontrada.Link,
                Compatibilidade = analise.Compatibilidade,
                Recomendacao = analise.Recomendacao,
                NivelVaga = analise.NivelVaga,
                Justificativa = analise.Justificativa
            };

            _context.Vagas.Add(vaga);

            quantidadeSalva++;
        }

        await _context.SaveChangesAsync();

        return quantidadeSalva;
    }

    private string NormalizarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return "";

        return texto
            .ToLowerInvariant()
            .Replace(" - pr", "")
            .Replace("- pr", "")
            .Trim();
    }

    private string LimparHtml(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return "";

        var semTags = Regex.Replace(
            texto,
            "<.*?>",
            " "
        );

        var decodificado =
            WebUtility.HtmlDecode(semTags);

        return Regex.Replace(
            decodificado,
            @"\s+",
            " "
        ).Trim();
    }
}