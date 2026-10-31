using PetFamily.Application.Dtos;
using PetFamily.Application.modules.CreateVolunteer;

namespace PetFamily.UnitTests;

public class CreateVolunteerValidatorTests
{
    private readonly CreateVolunteerValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_ReturnsNoErrors()
    {
        var request = CreateValidRequest();

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_InvalidEmail_ReturnsError(string email)
    {
        var request = CreateValidRequest() with { Email = email };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Email));
    }

    [Fact]
    public void Validate_ShortDescription_ReturnsError()
    {
        var request = CreateValidRequest() with { Description = "short" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Description));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_InvalidExperience_ReturnsError(int years)
    {
        var request = CreateValidRequest() with { YearsOfExperience = years };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.YearsOfExperience));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    public void Validate_InvalidPhone_ReturnsError(string phone)
    {
        var request = CreateValidRequest() with { PhoneNumber = phone };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.PhoneNumber));
    }

    [Fact]
    public void Validate_InvalidSocialLinkUrl_ReturnsError()
    {
        var request = CreateValidRequest() with
        {
            SocialLinks = [new SocialLinkDto("VK", "not-a-url")]
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "SocialLinks[0].Url");
    }

    private static CreateVolunteerRequest CreateValidRequest()
    {
        return new CreateVolunteerRequest(
            "Иван",
            "Иванов",
            "Иванович",
            "ivan@example.com",
            "Опытный волонтер с большим стажем",
            5,
            "+79991234567",
            null,
            null);
    }
}
