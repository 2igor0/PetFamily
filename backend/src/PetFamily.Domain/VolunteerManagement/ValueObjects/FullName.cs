using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteerManagement.ValueObjects;

public record FullName
{
    public string FirstName { get; }

    public string LastName { get; }

    public string? Patronymic { get; }

    private FullName()
    {
        FirstName = null!;
        LastName = null!;
    }

    private FullName(string firstName, string lastName, string? patronymic)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }

    public static Result<FullName, Error> Create(
        string? firstName,
        string? lastName,
        string? patronymic)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("full.name.first.name.is.invalid", "First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("full.name.last.name.is.invalid", "Last name cannot be empty.");

        var trimmedFirstName = firstName.Trim();
        var trimmedLastName = lastName.Trim();
        var trimmedPatronymic = string.IsNullOrWhiteSpace(patronymic) ? null : patronymic.Trim();

        if (trimmedFirstName.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "full.name.first.name.is.too.long",
                $"First name cannot exceed {Constants.MaxLowTextLength} characters.");

        if (trimmedLastName.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "full.name.last.name.is.too.long",
                $"Last name cannot exceed {Constants.MaxLowTextLength} characters.");

        if (trimmedPatronymic?.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "full.name.patronymic.is.too.long",
                $"Patronymic cannot exceed {Constants.MaxLowTextLength} characters.");

        return new FullName(trimmedFirstName, trimmedLastName, trimmedPatronymic);
    }
}
