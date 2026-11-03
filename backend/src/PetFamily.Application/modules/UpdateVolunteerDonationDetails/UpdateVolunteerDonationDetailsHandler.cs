using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Caching;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Application.modules.UpdateVolunteerDonationDetails;

public class UpdateVolunteerDonationDetailsHandler(
    IVolunteerRepository repository,
    ICacheInvalidator cacheInvalidator,
    ILogger<UpdateVolunteerDonationDetailsHandler> logger)
{
    public async Task<UnitResult<Error>> Execute(
        Guid volunteerId,
        UpdateVolunteerDonationDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (volunteerId == Guid.Empty)
            return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        var volunteer = await repository.GetById(
            IdVolunteer.Create(volunteerId),
            cancellationToken: cancellationToken);

        if (volunteer is null)
            return Error.NotFound("volunteer.not.found", "Volunteer was not found.");

        var donationDetails = new List<DonationDetails>();
        if (request.DonationDetails is not null)
        {
            foreach (var donation in request.DonationDetails)
            {
                var detailsResult = DonationDetails.Create(donation.Name, donation.Description);
                if (detailsResult.IsFailure)
                    return detailsResult.Error;

                donationDetails.Add(detailsResult.Value);
            }
        }

        var updateResult = volunteer.UpdateDonationDetails(donationDetails);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await repository.SaveChanges(cancellationToken);
        await cacheInvalidator.InvalidateVolunteerAsync(volunteerId, cancellationToken);

        logger.LogInformation("Volunteer {VolunteerId} donation details were updated", volunteerId);

        return UnitResult.Success<Error>();
    }
}
