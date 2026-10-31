using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.UnitTests;

public class VolunteerTests
{
    [Fact]
    public void Create_ValidData_ReturnsVolunteer()
    {
        var volunteer = TestData.CreateVolunteer();

        Assert.Equal("Иван", volunteer.FullName.FirstName);
        Assert.Equal(TestData.ValidEmail, volunteer.Email.Value);
        Assert.Equal(5, volunteer.YearsOfExperience.Value);
        Assert.False(volunteer.IsDeleted);
    }

    [Fact]
    public void AddPet_WithMatchingVolunteer_AddsPet()
    {
        var volunteer = TestData.CreateVolunteer();
        var pet = TestData.CreatePet(volunteer.Id);

        var result = volunteer.AddPet(pet);

        Assert.True(result.IsSuccess);
        Assert.Single(volunteer.Pets);
    }

    [Fact]
    public void AddPet_WithAnotherVolunteer_ReturnsFailure()
    {
        var volunteer = TestData.CreateVolunteer();
        var pet = TestData.CreatePet();

        var result = volunteer.AddPet(pet);

        Assert.True(result.IsFailure);
        Assert.Empty(volunteer.Pets);
    }

    [Fact]
    public void PetCounts_ReturnCorrectValues()
    {
        var volunteer = TestData.CreateVolunteer();

        volunteer.AddPet(TestData.CreatePet(volunteer.Id, HelpStatus.FoundHome));
        volunteer.AddPet(TestData.CreatePet(volunteer.Id, HelpStatus.FoundHome));
        volunteer.AddPet(TestData.CreatePet(volunteer.Id, HelpStatus.LookingForHome));
        volunteer.AddPet(TestData.CreatePet(volunteer.Id, HelpStatus.NeedsHelp));

        Assert.Equal(2, volunteer.GetAdoptedPetsCount());
        Assert.Equal(1, volunteer.GetPetsSeekingHomeCount());
        Assert.Equal(1, volunteer.GetPetsUnderTreatmentCount());
    }

    [Fact]
    public void Delete_MarksVolunteerAndPetsAsDeleted()
    {
        var volunteer = TestData.CreateVolunteer();
        var pet = TestData.CreatePet(volunteer.Id);
        volunteer.AddPet(pet);

        var result = volunteer.Delete();

        Assert.True(result.IsSuccess);
        Assert.True(volunteer.IsDeleted);
        Assert.NotNull(volunteer.DeletedAt);
        Assert.True(pet.IsDeleted);
    }

    [Fact]
    public void AddPet_ToDeletedVolunteer_ReturnsFailure()
    {
        var volunteer = TestData.CreateVolunteer();
        volunteer.Delete();

        var result = volunteer.AddPet(TestData.CreatePet(volunteer.Id));

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void UpdateSocialNetworks_ReplacesExistingList()
    {
        var volunteer = TestData.CreateVolunteer();
        var network = SocialNetwork.Create("VK", "https://vk.com/volunteer").Value;

        var result = volunteer.UpdateSocialNetworks([network]);

        Assert.True(result.IsSuccess);
        Assert.Single(volunteer.SocialNetworks);
        Assert.Equal("VK", volunteer.SocialNetworks[0].Name);
    }

    [Fact]
    public void UpdateMainInfo_ChangesMainFields()
    {
        var volunteer = TestData.CreateVolunteer();
        var fullName = FullName.Create("Пётр", "Петров", null).Value;
        var email = PetFamily.Domain.Shared.Email.Create("petr@example.com").Value;
        var description = PetFamily.Domain.Shared.Description.Create("Новое описание волонтера").Value;
        var experience = Experience.Create(10).Value;
        var phone = PetFamily.Domain.Shared.PhoneNumber.Create("+79990000000").Value;

        var result = volunteer.UpdateMainInfo(fullName, email, description, experience, phone);

        Assert.True(result.IsSuccess);
        Assert.Equal("Пётр", volunteer.FullName.FirstName);
        Assert.Equal("petr@example.com", volunteer.Email.Value);
        Assert.Equal(10, volunteer.YearsOfExperience.Value);
    }
}
