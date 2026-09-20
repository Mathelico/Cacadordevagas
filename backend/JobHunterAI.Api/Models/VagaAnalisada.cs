namespace JobHunterAI.Api.Models;

public class VagaAnalisada
{
    public VagaEncontrada Vaga { get; set; } = new();

    public AnaliseVagaResponse Analise { get; set; } = new();
}