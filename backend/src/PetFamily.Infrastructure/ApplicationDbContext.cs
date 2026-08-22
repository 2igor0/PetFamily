using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetFamily.Domain;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.Infrastructure;

public class ApplicationDbContext : DbContext
{
    private const string Database = "Database";

    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Volunteer> Volunteers => Set<Volunteer>();
    public DbSet<Species> Species => Set<Species>();
    

    private readonly IConfiguration _configuration;

    // Оставляем ТОЛЬКО ОДИН конструктор!
    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString(Database));
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}