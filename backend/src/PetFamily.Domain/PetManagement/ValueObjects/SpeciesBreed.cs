using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record SpeciesBreed
{
    public IdSpecies SpeciesId { get; }

    public IdBreed BreedId { get; }

    private SpeciesBreed()
    {
        SpeciesId = IdSpecies.Empty();
        BreedId = IdBreed.Empty();
    }

    private SpeciesBreed(IdSpecies speciesId, IdBreed breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public static Result<SpeciesBreed, Error> Create(IdSpecies speciesId, IdBreed breedId)
    {
        if (speciesId.Value == Guid.Empty)
            return Error.Validation("species.id.is.invalid", "Species id cannot be empty.");

        if (breedId.Value == Guid.Empty)
            return Error.Validation("breed.id.is.invalid", "Breed id cannot be empty.");

        return new SpeciesBreed(speciesId, breedId);
    }
}
