using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Infrastructure.configurations;

public class BreedConfiguration : IEntityTypeConfiguration<Breed>
{
    private static readonly IdSpecies DogSpeciesId =
        IdSpecies.Create(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    private static readonly IdSpecies CatSpeciesId =
        IdSpecies.Create(Guid.Parse("22222222-2222-2222-2222-222222222222"));

    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("breeds");

        builder.HasKey(breed => breed.Id);

        builder.Property(breed => breed.Value)
            .HasColumnName("value")
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.Property(breed => breed.SpeciesId)
            .HasColumnName("species_id")
            .IsRequired();

        builder.HasIndex(breed => new { breed.SpeciesId, breed.Value })
            .IsUnique()
            .HasDatabaseName("ix_breeds_species_id_value");

        builder.HasData(
            new
            {
                Id = IdBreed.Create(Guid.Parse("11111111-1111-1111-1111-111111111101")),
                SpeciesId = DogSpeciesId,
                Value = "Лабрадор"
            },
            new
            {
                Id = IdBreed.Create(Guid.Parse("11111111-1111-1111-1111-111111111102")),
                SpeciesId = DogSpeciesId,
                Value = "Немецкая овчарка"
            },
            new
            {
                Id = IdBreed.Create(Guid.Parse("11111111-1111-1111-1111-111111111103")),
                SpeciesId = DogSpeciesId,
                Value = "Бигль"
            },
            new
            {
                Id = IdBreed.Create(Guid.Parse("22222222-2222-2222-2222-222222222201")),
                SpeciesId = CatSpeciesId,
                Value = "Британская короткошерстная"
            },
            new
            {
                Id = IdBreed.Create(Guid.Parse("22222222-2222-2222-2222-222222222202")),
                SpeciesId = CatSpeciesId,
                Value = "Мейн-кун"
            },
            new
            {
                Id = IdBreed.Create(Guid.Parse("22222222-2222-2222-2222-222222222203")),
                SpeciesId = CatSpeciesId,
                Value = "Шотландская вислоухая"
            });
    }
}
