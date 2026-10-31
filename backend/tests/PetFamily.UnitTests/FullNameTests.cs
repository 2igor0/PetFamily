using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.UnitTests;

public class FullNameTests
{
    [Fact]
    public void Create_ValidName_TrimsValues()
    {
        var result = FullName.Create("  Иван ", " Иванов  ", "  Иванович ");

        Assert.True(result.IsSuccess);
        Assert.Equal("Иван", result.Value.FirstName);
        Assert.Equal("Иванов", result.Value.LastName);
        Assert.Equal("Иванович", result.Value.Patronymic);
    }

    [Fact]
    public void Create_EmptyPatronymic_ReturnsNull()
    {
        var result = FullName.Create("Иван", "Иванов", "   ");

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Patronymic);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyFirstName_ReturnsFailure(string firstName)
    {
        var result = FullName.Create(firstName, "Иванов", null);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyLastName_ReturnsFailure(string lastName)
    {
        var result = FullName.Create("Иван", lastName, null);

        Assert.True(result.IsFailure);
    }
}
