using PetFamily.Domain.Shared;

namespace PetFamily.UnitTests;

public class EmailTests
{
    [Theory]
    [InlineData("ivan@example.com")]
    [InlineData("IVAN@EXAMPLE.COM")]
    [InlineData("ivan.petrov+tag@example.org")]
    public void Create_ValidEmail_ReturnsSuccess(string email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsSuccess);
        Assert.Equal(email.Trim().ToLowerInvariant(), result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("ivan@")]
    [InlineData("ivan@example")]
    public void Create_InvalidEmail_ReturnsFailure(string email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_TooLongEmail_ReturnsFailure()
    {
        var email = new string('a', Constants.MaxEmailLength) + "@example.com";

        var result = Email.Create(email);

        Assert.True(result.IsFailure);
    }
}
