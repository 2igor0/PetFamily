using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record PhoneNumber
{
    private const string Pattern = @"^\+?[0-9\s\-\(\)]{5,20}$";

    public string Value { get; }

    private PhoneNumber()
    {
        Value = null!;
    }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber, Error> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Error.Validation("phone.number.is.invalid", "Phone number cannot be empty.");

        var trimmedPhoneNumber = phoneNumber.Trim();

        if (trimmedPhoneNumber.Length > Constants.MaxPhoneLength)
            return Error.Validation(
                "phone.number.is.too.long",
                $"Phone number cannot exceed {Constants.MaxPhoneLength} characters.");

        if (!Regex.IsMatch(trimmedPhoneNumber, Pattern))
            return Error.Validation("phone.number.is.invalid", "Phone number has an invalid format.");

        var cleanPhoneNumber = Regex.Replace(trimmedPhoneNumber, @"[^\d+]", string.Empty);

        return new PhoneNumber(cleanPhoneNumber);
    }
}
