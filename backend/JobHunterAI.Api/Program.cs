using JobHunterAI.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a Controllers
builder.Services.AddControllers();

// Registra o serviço responsável pela comunicação com a OpenAI
builder.Services.AddHttpClient<OpenAIService>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Configura o OpenAPI apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Mapeia os endpoints criados nos Controllers
app.MapControllers();

app.Run();