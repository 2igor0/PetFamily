using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.UnitTests;

internal static class TestData
{
    public static readonly Guid DogSpeciesId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid LabradorBreedId =
        Guid.Parse("11111111-1111-1111-1111-111111111101");

    public const string ValidEmail = "ivan@example.com";
    public const string ValidPhone = "+79991234567";

    public static Volunteer CreateVolunteer()
    {
        var result = Volunteer.Create(
            IdVolunteer.CreateNew(),
            "Иван",
            "Иванов",
            "Иванович",
            ValidEmail,
            "Опытный волонтер с большим стажем",
            5,
            ValidPhone);

        return result.Value;
    }

    public static Pet CreatePet(
        IdVolunteer? volunteerId = null,
        HelpStatus helpStatus = HelpStatus.LookingForHome,
        DateOnly? birthDate = null,
        IEnumerable<PetPhoto>? photos = null)
    {
        var result = Pet.Create(
            volunteerId ?? IdVolunteer.CreateNew(),
            "Барсик",
            DogSpeciesId,
            LabradorBreedId,
            "Добрый и ласковый питомец",
            "Рыжий",
            "Здоров",
            "Россия",
            "Москва",
            "Ленина",
            10,
            5,
            40,
            ValidPhone,
            false,
            birthDate ?? new DateOnly(2020, 1, 1),
            true,
            helpStatus,
            null,
            photos);

        return result.Value;
    }
}
