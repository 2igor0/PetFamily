using PetFamily.Domain.PetManagement.ValueObjects;

namespace PetFamily.UnitTests;

public class AddressTests
{
    [Fact]
    public void Create_ValidAddress_ReturnsSuccess()
    {
        var result = Address.Create("Россия", "Москва", "Ленина", 10);

        Assert.True(result.IsSuccess);
        Assert.Equal("Россия", result.Value.Country);
        Assert.Equal("Москва", result.Value.City);
        Assert.Equal("Ленина", result.Value.Street);
        Assert.Equal(10, result.Value.HouseNumber);
    }

    [Theory]
    [InlineData("", "Москва", "Ленина")]
    [InlineData("Россия", "", "Ленина")]
    [InlineData("Россия", "Москва", "")]
    public void Create_EmptyRequiredField_ReturnsFailure(string country, string city, string street)
    {
        var result = Address.Create(country, city, street, 10);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_InvalidHouseNumber_ReturnsFailure(int houseNumber)
    {
        var result = Address.Create("Россия", "Москва", "Ленина", houseNumber);

        Assert.True(result.IsFailure);
    }
}
