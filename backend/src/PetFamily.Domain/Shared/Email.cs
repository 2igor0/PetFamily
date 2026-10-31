using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email()
    {
        Value = null!;
    }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email, Error> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("email.is.invalid", "Email cannot be empty.");

        var trimmedEmail = email.Trim().ToLowerInvariant();

        if (trimmedEmail.Length > Constants.MaxEmailLength)
            return Error.Validation(
                "email.is.too.long",
                $"Email cannot exceed {Constants.MaxEmailLength} characters.");

        if (!EmailRegex.IsMatch(trimmedEmail))
            return Error.Validation("email.is.invalid", "Email has an invalid format.");

        return new Email(trimmedEmail);
    }
}
