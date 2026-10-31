using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record PhysicalAttributes
{
    public int Value { get; }

    private PhysicalAttributes()
    {
    }

    private PhysicalAttributes(int value)
    {
        Value = value;
    }

    public static Result<PhysicalAttributes, Error> Create(int value)
    {
        if (value < 0 || value > Constants.MaxPhysicalAttribute)
            return Error.Validation(
                "physical.attribute.is.invalid",
                $"Value must be between 0 and {Constants.MaxPhysicalAttribute}.");

        return new PhysicalAttributes(value);
    }
}
