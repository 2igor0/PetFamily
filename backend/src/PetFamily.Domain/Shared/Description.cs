using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record Description
{
    public string Value { get; }

    private Description()
    {
        Value = null!;
    }

    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description, Error> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Error.Validation("description.is.invalid", "Description cannot be empty.");

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > Constants.MaxHighDescription)
            return Error.Validation(
                "description.is.too.long",
                $"Description cannot exceed {Constants.MaxHighDescription} characters.");

        return new Description(trimmedDescription);
    }
}
