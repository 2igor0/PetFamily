using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Domain.PetManagement.SpeciesAndBreed;

public class Breed : Shared.Entity<IdBreed>
{
    private Breed(string value, IdBreed id) : base(id) //Для обновления модели должны быть отдельные методы.
    {
        Value = value;
    }
    
    public string Value { get; private set; }

    public static Result<Breed, string> Create(string input, IdBreed id)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Breed cannot be empty.";

        var trimmedInput = input.Trim();

        if (trimmedInput.Length > Constants.MaxLowTextLength)
            return $"Breed name cannot exceed {Constants.MaxLowTextLength} characters.";
        
        if (trimmedInput.Any(char.IsDigit))
            return "Breed cannot contain numbers.";

        return new Breed(trimmedInput, id);
    }
}