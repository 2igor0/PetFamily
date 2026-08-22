using PetFamily.Application.Dtos;

namespace PetFamily.Application.modules.CreateVolunteer;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string? Patronymic,
    string Description,
    int YearsOfExperience,
    string PhoneNumber,
    IEnumerable<SocialLinkDto>? SocialLinks,
    IEnumerable<DonationsRequisites>? DonationsRequisites);   