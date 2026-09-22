namespace JobHunterAI.Api.Models;

public class Vaga
{
    public int Id { get; set; }

    public string Cargo { get; set; } = "";

    public string Empresa { get; set; } = "";

    public string Descricao { get; set; } = "";

    public string Link { get; set; } = "";

    public int Compatibilidade { get; set; }

    public string Recomendacao { get; set; } = "";

    public string NivelVaga { get; set; } = "";

    public string Justificativa { get; set; } = "";

    public bool Visualizada { get; set; } = false;

    public DateTime DataAnalise { get; set; } = DateTime.UtcNow;
}