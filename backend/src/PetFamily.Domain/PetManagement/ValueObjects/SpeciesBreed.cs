using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record SpeciesBreed
{
    public Species SpeciesId { get; }
    public Guid BreedId { get; }

    // Для EF Core / сериализации
    private SpeciesBreed()
    {
        
    }

    private SpeciesBreed(Species species, Guid breedId)
    {
        SpeciesId = species;
        BreedId = breedId;
    }

    public static Result<SpeciesBreed, Error> Create(Species species, Guid breedId)
    {
        // валидация

        return new SpeciesBreed(species, breedId);
    }
}