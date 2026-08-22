using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record Description
{
    public string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description, Error> Create(string description) //
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Error.Validation("description.is.invalid", "description is  null or empty");
        }

        if (description.Length > Constants.MaxHighDescription)
        {
            return Error.Validation("description.is.invalid", "description is too long");
        }
        
        return new Description(description);
    }
}