namespace JobHunterAI.Api.Models;

public class VagaEncontrada
{
    public string IdExterno { get; set; } = "";

    public string Titulo { get; set; } = "";

    public string Empresa { get; set; } = "";

    public string Localizacao { get; set; } = "";

    public string Descricao { get; set; } = "";

    public string Link { get; set; } = "";

    public string Fonte { get; set; } = "";

    public DateTime? DataPublicacao { get; set; }
}