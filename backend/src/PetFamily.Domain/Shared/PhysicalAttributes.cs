using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record PhysicalAttributes
{
    public int Value { get; }
    
    private PhysicalAttributes(int value)
    {
        Value = value;
    }
    
    public static Result<PhysicalAttributes, Error> Create(int input)
    {
        if (input < 0 || input > 1000)
            return Error.Validation("input.is.invalid", "Input must be between 0 and 1000");

        return new PhysicalAttributes(input);
    }
}