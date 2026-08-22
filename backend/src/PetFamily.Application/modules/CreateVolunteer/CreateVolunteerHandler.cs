using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.Application.modules.CreateVolunteer;

public class CreateVolunteerHandler(IVolunteerRepository repository)
{
    public async Task<Result<Guid, Error>> Execute(CreateVolunteerRequest request, CancellationToken token = default)
    {
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumber.IsFailure)
            return phoneNumber.Error; // ну тут как-то над выводить именно Error для валидации и тд

        var id = IdVolunteer.CreateNewId();
        
        var fullName = FullName.Create(request.FirstName, request.LastName, request.Patronymic);
        if (fullName.IsFailure)
            return fullName.Error;
        
        var description = Description.Create(request.Description);
        if (description.IsFailure)
            return description.Error; 
        
        var yearsOfExperience = PhysicalAttributes.Create(request.YearsOfExperience);
        if (yearsOfExperience.IsFailure)
            return yearsOfExperience.Error;

        var socialLinks = new List<SocialNetwork>();
        if (request.SocialLinks != null)
            foreach (var requestSocialLink in request.SocialLinks)
            {
                var socialLink = SocialNetwork.Create(requestSocialLink.Name, requestSocialLink.Url);
                if (socialLink.IsFailure)
                    return socialLink.Error;

                socialLinks.Add(socialLink.Value);
            }
    
        var donationsRequisites = new List<DonationDetails>();
        if (request.DonationsRequisites != null)
            foreach (var donationsRequisite in request.DonationsRequisites)
            {
                var validDonationsRequisite = DonationDetails.Create(donationsRequisite.Name, donationsRequisite.Description); 
                if(validDonationsRequisite.IsFailure)
                    return validDonationsRequisite.Error;

                donationsRequisites.Add(validDonationsRequisite.Value);
            }
        
        // тут проверка что ли хз, вроде надо в контроллере
        var volunteer = Volunteer.Create(fullName.Value, description.Value, yearsOfExperience.Value, phoneNumber.Value, id, socialLinks, donationsRequisites).Value;
        await repository.Add(volunteer, token);
        
        return volunteer.Id.Value;
    }
}