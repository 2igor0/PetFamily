using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.Application.modules.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerRepository repository,
    ILogger<CreateVolunteerHandler> logger)
{
    public async Task<Result<Guid, Error>> Execute(
        CreateVolunteerRequest request,
        CancellationToken token = default)
    {
        var socialNetworks = new List<SocialNetwork>();
        if (request.SocialLinks is not null)
        {
            foreach (var requestSocialLink in request.SocialLinks)
            {
                var socialNetwork = SocialNetwork.Create(requestSocialLink.Name, requestSocialLink.Url);
                if (socialNetwork.IsFailure)
                    return socialNetwork.Error;

                socialNetworks.Add(socialNetwork.Value);
            }
        }

        var donationDetails = new List<DonationDetails>();
        if (request.DonationsRequisites is not null)
        {
            foreach (var requestDonation in request.DonationsRequisites)
            {
                var details = DonationDetails.Create(requestDonation.Name, requestDonation.Description);
                if (details.IsFailure)
                    return details.Error;

                donationDetails.Add(details.Value);
            }
        }

        var volunteerResult = Volunteer.Create(
            IdVolunteer.CreateNew(),
            request.FirstName,
            request.LastName,
            request.Patronymic,
            request.Email,
            request.Description,
            request.YearsOfExperience,
            request.PhoneNumber,
            socialNetworks,
            donationDetails);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error;

        var volunteer = volunteerResult.Value;

        await repository.Add(volunteer, token);

        logger.LogInformation(
            "Volunteer {VolunteerId} was created successfully",
            volunteer.Id.Value);

        return volunteer.Id.Value;
    }
}
