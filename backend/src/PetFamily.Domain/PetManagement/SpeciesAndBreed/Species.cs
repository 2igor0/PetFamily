using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;


namespace PetFamily.Domain.PetManagement.SpeciesAndBreed;

public class Species : Shared.Entity<IdSpecies>
{
    //ef core
    public Species() : base(IdSpecies.Empty()) //Для обновления модели должны быть отдельные методы.
    {
    }
    
    private Species(string value, IdSpecies id) : base(id) 
    {
        Value = value;
    }
    
    private readonly List<Breed> _breeds = [];
    
    public IReadOnlyList<Breed> Breeds => _breeds;
    
    public string Value { get; private set; }

    public static Result<Species, string> Create(string input, IdSpecies id)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Specie cannot be empty.";

        if (input.Length > Constants.MaxLowTextLength)
            return $"Breed name cannot exceed {Constants.MaxLowTextLength} characters.";
        
        if (input.Any(char.IsDigit))
            return "Breed cannot contain numbers.";

        return new Species(input, id);
    }
    
    public Result AddBreed(Breed breed)
    {
        if (_breeds.Any(b => b.Value.Equals(breed.Value, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure("Breed already exists in this species.");

        _breeds.Add(breed);
        return Result.Success();
    }
}