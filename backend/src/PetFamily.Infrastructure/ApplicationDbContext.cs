using Microsoft.EntityFrameworkCore;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;
using PetFamily.Infrastructure.Converters;

namespace PetFamily.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<Volunteer> Volunteers => Set<Volunteer>();

    public DbSet<Species> Species => Set<Species>();

    public DbSet<Breed> Breeds => Set<Breed>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<IdVolunteer>().HaveConversion<IdVolunteerConverter>();
        configurationBuilder.Properties<IdPet>().HaveConversion<IdPetConverter>();
        configurationBuilder.Properties<IdSpecies>().HaveConversion<IdSpeciesConverter>();
        configurationBuilder.Properties<IdBreed>().HaveConversion<IdBreedConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
