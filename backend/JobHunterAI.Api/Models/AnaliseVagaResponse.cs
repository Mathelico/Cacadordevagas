namespace JobHunterAI.Api.Models;

public class AnaliseVagaResponse
{
    public int Compatibilidade { get; set; }

    public string Recomendacao { get; set; } = "";

    public List<string> PontosFortes { get; set; } = new();

    public List<string> ConhecimentosAusentes { get; set; } = new();

    public string NivelVaga { get; set; } = "";

    public string Justificativa { get; set; } = "";
}