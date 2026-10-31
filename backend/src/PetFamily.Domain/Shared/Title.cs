using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record Title
{
    public string Value { get; }

    private Title()
    {
        Value = null!;
    }

    private Title(string value)
    {
        Value = value;
    }

    public static Result<Title, Error> Create(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Error.Validation("title.is.invalid", "Title cannot be empty.");

        var trimmedTitle = title.Trim();

        if (trimmedTitle.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "title.is.too.long",
                $"Title cannot exceed {Constants.MaxLowTextLength} characters.");

        return new Title(trimmedTitle);
    }
}
