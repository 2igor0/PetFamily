using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.UnitTests;

public class PetTests
{
    [Fact]
    public void Create_ValidData_ReturnsPet()
    {
        var volunteerId = IdVolunteer.CreateNew();

        var result = Pet.Create(
            volunteerId,
            "Барсик",
            TestData.DogSpeciesId,
            TestData.LabradorBreedId,
            "Добрый и ласковый питомец",
            "Рыжий",
            "Здоров",
            "Россия",
            "Москва",
            "Ленина",
            10,
            5,
            40,
            TestData.ValidPhone,
            false,
            new DateOnly(2020, 1, 1),
            true,
            HelpStatus.LookingForHome);

        Assert.True(result.IsSuccess);
        Assert.Equal(volunteerId, result.Value.VolunteerId);
        Assert.Equal("Барсик", result.Value.Name.Value);
        Assert.Equal(HelpStatus.LookingForHome, result.Value.HelpStatus);
        Assert.Empty(result.Value.DonationDetails);
        Assert.Empty(result.Value.Photos);
    }

    [Fact]
    public void Create_FutureBirthDate_ReturnsFailure()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var result = Pet.Create(
            IdVolunteer.CreateNew(),
            "Барсик",
            TestData.DogSpeciesId,
            TestData.LabradorBreedId,
            "Добрый и ласковый питомец",
            "Рыжий",
            "Здоров",
            "Россия",
            "Москва",
            "Ленина",
            10,
            5,
            40,
            TestData.ValidPhone,
            false,
            futureDate,
            true,
            HelpStatus.LookingForHome);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_MoreThanOneMainPhoto_ReturnsFailure()
    {
        var photos = new[]
        {
            PetPhoto.Create("photo-1.jpg", true).Value,
            PetPhoto.Create("photo-2.jpg", true).Value
        };

        var result = Pet.Create(
            IdVolunteer.CreateNew(),
            "Барсик",
            TestData.DogSpeciesId,
            TestData.LabradorBreedId,
            "Добрый и ласковый питомец",
            "Рыжий",
            "Здоров",
            "Россия",
            "Москва",
            "Ленина",
            10,
            5,
            40,
            TestData.ValidPhone,
            false,
            new DateOnly(2020, 1, 1),
            true,
            HelpStatus.LookingForHome,
            photos: photos);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_NoMainPhoto_MakesFirstPhotoMain()
    {
        var photos = new[]
        {
            PetPhoto.Create("photo-1.jpg", false).Value,
            PetPhoto.Create("photo-2.jpg", false).Value
        };

        var result = Pet.Create(
            IdVolunteer.CreateNew(),
            "Барсик",
            TestData.DogSpeciesId,
            TestData.LabradorBreedId,
            "Добрый и ласковый питомец",
            "Рыжий",
            "Здоров",
            "Россия",
            "Москва",
            "Ленина",
            10,
            5,
            40,
            TestData.ValidPhone,
            false,
            new DateOnly(2020, 1, 1),
            true,
            HelpStatus.LookingForHome,
            photos: photos);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Photos[0].IsMain);
        Assert.False(result.Value.Photos[1].IsMain);
    }

    [Fact]
    public void AddPhoto_WhenNewPhotoIsMain_RemovesMainFromPrevious()
    {
        var pet = TestData.CreatePet(
            photos: [PetPhoto.Create("photo-1.jpg", true).Value]);

        var result = pet.AddPhoto(PetPhoto.Create("photo-2.jpg", true).Value);

        Assert.True(result.IsSuccess);
        Assert.False(pet.Photos[0].IsMain);
        Assert.True(pet.Photos[1].IsMain);
    }

    [Fact]
    public void RemovePhoto_WhenMainPhotoRemoved_MakesNextPhotoMain()
    {
        var pet = TestData.CreatePet(
            photos:
            [
                PetPhoto.Create("photo-1.jpg", true).Value,
                PetPhoto.Create("photo-2.jpg", false).Value
            ]);

        var result = pet.RemovePhoto("photo-1.jpg");

        Assert.True(result.IsSuccess);
        Assert.Single(pet.Photos);
        Assert.True(pet.Photos[0].IsMain);
    }

    [Fact]
    public void Delete_MarksPetAsDeleted()
    {
        var pet = TestData.CreatePet();

        var result = pet.Delete();

        Assert.True(result.IsSuccess);
        Assert.True(pet.IsDeleted);
        Assert.NotNull(pet.DeletedAt);
    }
}
