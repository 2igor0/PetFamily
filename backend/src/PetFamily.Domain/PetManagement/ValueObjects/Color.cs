using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record Color
{
    public string Value { get; }

    private Color()
    {
        Value = null!;
    }

    private Color(string value)
    {
        Value = value;
    }

    public static Result<Color, Error> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Error.Validation("color.is.invalid", "Color cannot be empty.");

        var trimmedInput = input.Trim();

        if (trimmedInput.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "color.is.too.long",
                $"Color cannot exceed {Constants.MaxLowTextLength} characters.");

        return new Color(trimmedInput);
    }
}
