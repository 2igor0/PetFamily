using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record PhoneNumber
{
    public string Value { get; }
    private const string Pattern = @"^\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$";

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber, Error> Create(string phoneNumber) 
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Error.Validation("value.is.invalid", "Phone number is null or empty");

        if (!Regex.IsMatch(phoneNumber, Pattern))
            return Error.Validation("value.is.invalid", "Phone number is not a valid phone number");
        
        var cleanPhone = Regex.Replace(phoneNumber, @"[^\d+]", "");
        
        return new PhoneNumber(cleanPhone);
    }
}