using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record Address
{
    public string Country { get; }

    public string City { get; }

    public string Street { get; }

    public int HouseNumber { get; }

    private Address()
    {
        Country = null!;
        City = null!;
        Street = null!;
    }

    private Address(string country, string city, string street, int houseNumber)
    {
        Country = country;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
    }

    public static Result<Address, Error> Create(
        string? country,
        string? city,
        string? street,
        int houseNumber)
    {
        if (string.IsNullOrWhiteSpace(country))
            return Error.Validation("address.country.is.invalid", "Country cannot be empty.");

        if (string.IsNullOrWhiteSpace(city))
            return Error.Validation("address.city.is.invalid", "City cannot be empty.");

        if (string.IsNullOrWhiteSpace(street))
            return Error.Validation("address.street.is.invalid", "Street cannot be empty.");

        if (houseNumber <= 0)
            return Error.Validation("address.house.number.is.invalid", "House number must be positive.");

        return new Address(
            country.Trim(),
            city.Trim(),
            street.Trim(),
            houseNumber);
    }
}
