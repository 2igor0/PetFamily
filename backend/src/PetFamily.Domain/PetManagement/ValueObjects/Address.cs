namespace PetFamily.Domain.PetManagement.ValueObjects;

public record Address
{
    public string Country { get; }
    public string City { get; }
    public string Street { get; }
    public int HouseNumber { get; }

    private Address(string country, string city, string street, int houseNumber)
    {
        Country = country;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
    }

    public static Address Create(string country, string city, string street, int houseNumber)
    {
        // валидация
        
        return new Address(country, city, street, houseNumber);
    }
}