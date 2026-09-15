using JobHunterAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobHunterAI.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vaga> Vagas { get; set; }
}