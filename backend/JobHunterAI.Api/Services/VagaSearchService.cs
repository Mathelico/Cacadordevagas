using JobHunterAI.Api.Models;

namespace JobHunterAI.Api.Services;

public class VagaSearchService
{
    private readonly RemotiveService _remotiveService;
    private readonly JoobleService _joobleService;

    public VagaSearchService(
        RemotiveService remotiveService,
        JoobleService joobleService)
    {
        _remotiveService = remotiveService;
        _joobleService = joobleService;
    }

    public async Task<object> BuscarTodas()
    {
        var vagasRemotive = await _remotiveService.BuscarVagas();
        var vagasJooble = await _joobleService.BuscarVagas();

        return new
        {
            remotive = vagasRemotive,
            jooble = vagasJooble
        };
    }
}