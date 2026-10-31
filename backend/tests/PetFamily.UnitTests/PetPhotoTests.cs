using PetFamily.Domain.PetManagement.ValueObjects;

namespace PetFamily.UnitTests;

public class PetPhotoTests
{
    [Fact]
    public void Create_ValidPhoto_ReturnsSuccess()
    {
        var result = PetPhoto.Create("pets/photo.jpg", true);

        Assert.True(result.IsSuccess);
        Assert.Equal("pets/photo.jpg", result.Value.Path);
        Assert.True(result.Value.IsMain);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyPath_ReturnsFailure(string path)
    {
        var result = PetPhoto.Create(path, false);

        Assert.True(result.IsFailure);
    }
}
