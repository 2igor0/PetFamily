using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Domain.PetManagement.SpeciesAndBreed;

public class Species : Shared.Entity<IdSpecies>
{
    private readonly List<Breed> _breeds = [];

    // EF Core
    private Species() : base(IdSpecies.Empty())
    {
        Value = null!;
    }

    private Species(string value, IdSpecies id) : base(id)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public IReadOnlyList<Breed> Breeds => _breeds;

    public static Result<Species, Error> Create(string? input, IdSpecies id)
    {
        if (id.Value == Guid.Empty)
            return Error.Validation("species.id.is.invalid", "Species id cannot be empty.");

        if (string.IsNullOrWhiteSpace(input))
            return Error.Validation("species.name.is.invalid", "Species name cannot be empty.");

        var trimmedInput = input.Trim();

        if (trimmedInput.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "species.name.is.too.long",
                $"Species name cannot exceed {Constants.MaxLowTextLength} characters.");

        if (trimmedInput.Any(char.IsDigit))
            return Error.Validation("species.name.is.invalid", "Species name cannot contain digits.");

        return new Species(trimmedInput, id);
    }

    public UnitResult<Error> AddBreed(Breed? breed)
    {
        if (breed is null)
            return Error.Validation("breed.is.invalid", "Breed cannot be null.");

        if (breed.SpeciesId != Id)
            return Error.Validation("breed.species.id.is.invalid", "Breed belongs to another species.");

        if (_breeds.Any(existingBreed =>
                existingBreed.Value.Equals(breed.Value, StringComparison.OrdinalIgnoreCase)))
        {
            return Error.Conflict("breed.already.exists", "Breed already exists in this species.");
        }

        _breeds.Add(breed);

        return UnitResult.Success<Error>();
    }
}
