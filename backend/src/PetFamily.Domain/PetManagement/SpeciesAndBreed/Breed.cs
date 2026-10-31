using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Domain.PetManagement.SpeciesAndBreed;

public class Breed : Shared.Entity<IdBreed>
{
    // EF Core
    private Breed() : base(IdBreed.Empty())
    {
        SpeciesId = IdSpecies.Empty();
        Value = null!;
    }

    private Breed(string value, IdBreed id, IdSpecies speciesId) : base(id)
    {
        Value = value;
        SpeciesId = speciesId;
    }

    public string Value { get; private set; }

    public IdSpecies SpeciesId { get; private set; }

    public static Result<Breed, Error> Create(string? input, IdBreed id, IdSpecies speciesId)
    {
        if (id.Value == Guid.Empty)
            return Error.Validation("breed.id.is.invalid", "Breed id cannot be empty.");

        if (speciesId.Value == Guid.Empty)
            return Error.Validation("breed.species.id.is.invalid", "Species id cannot be empty.");

        if (string.IsNullOrWhiteSpace(input))
            return Error.Validation("breed.name.is.invalid", "Breed name cannot be empty.");

        var trimmedInput = input.Trim();

        if (trimmedInput.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "breed.name.is.too.long",
                $"Breed name cannot exceed {Constants.MaxLowTextLength} characters.");

        if (trimmedInput.Any(char.IsDigit))
            return Error.Validation("breed.name.is.invalid", "Breed name cannot contain digits.");

        return new Breed(trimmedInput, id, speciesId);
    }
}
