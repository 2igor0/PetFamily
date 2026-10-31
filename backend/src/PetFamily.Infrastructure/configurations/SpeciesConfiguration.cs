using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Infrastructure.configurations;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.ToTable("species");

        builder.HasKey(species => species.Id);

        builder.Property(species => species.Value)
            .HasColumnName("value")
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.HasIndex(species => species.Value)
            .IsUnique()
            .HasDatabaseName("ix_species_value");

        builder.HasMany(species => species.Breeds)
            .WithOne()
            .HasForeignKey(breed => breed.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(species => species.Breeds)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(
            new
            {
                Id = IdSpecies.Create(Guid.Parse("11111111-1111-1111-1111-111111111111")),
                Value = "Собака"
            },
            new
            {
                Id = IdSpecies.Create(Guid.Parse("22222222-2222-2222-2222-222222222222")),
                Value = "Кошка"
            });
    }
}
