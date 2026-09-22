using JobHunterAI.Api.Services;
using JobHunterAI.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura o PostgreSQL com Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Adiciona suporte a Controllers
builder.Services.AddControllers();

// Registra o serviço responsável pela comunicação com a OpenAI
builder.Services.AddHttpClient<OpenAIService>();

builder.Services.AddHttpClient<RemotiveService>();

builder.Services.AddHttpClient<JoobleService>();

builder.Services.AddScoped<VagaSearchService>();

// OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");

// Configura o OpenAPI apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Mapeia os endpoints criados nos Controllers
app.MapControllers();

app.Run();