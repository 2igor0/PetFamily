using PetFamily.Application.Dtos;
using PetFamily.Application.FileStorage;

namespace PetFamily.Application.modules.UpdatePet;

public record UpdatePetRequest(
    string Name,
    Guid SpeciesId,
    Guid BreedId,
    string Description,
    string Color,
    string HealthInfoDescription,
    string Country,
    string City,
    string Street,
    int HouseNumber,
    int Weight,
    int Height,
    string OwnerPhoneNumber,
    bool IsNeutered,
    DateOnly BirthDate,
    bool IsVaccinated,
    string HelpStatus,
    IEnumerable<DonationsRequisites>? DonationDetails,
    IEnumerable<UploadFileDto>? Photos);
