using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Caching;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.Application.modules.UpdateVolunteerSocialNetworks;

public class UpdateVolunteerSocialNetworksHandler(
    IVolunteerRepository repository,
    ICacheInvalidator cacheInvalidator,
    ILogger<UpdateVolunteerSocialNetworksHandler> logger)
{
    public async Task<UnitResult<Error>> Execute(
        Guid volunteerId,
        UpdateVolunteerSocialNetworksRequest request,
        CancellationToken cancellationToken = default)
    {
        if (volunteerId == Guid.Empty)
            return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        var volunteer = await repository.GetById(
            IdVolunteer.Create(volunteerId),
            cancellationToken: cancellationToken);

        if (volunteer is null)
            return Error.NotFound("volunteer.not.found", "Volunteer was not found.");

        var socialNetworks = new List<SocialNetwork>();
        if (request.SocialLinks is not null)
        {
            foreach (var socialLink in request.SocialLinks)
            {
                var socialNetworkResult = SocialNetwork.Create(socialLink.Name, socialLink.Url);
                if (socialNetworkResult.IsFailure)
                    return socialNetworkResult.Error;

                socialNetworks.Add(socialNetworkResult.Value);
            }
        }

        var updateResult = volunteer.UpdateSocialNetworks(socialNetworks);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await repository.SaveChanges(cancellationToken);
        await cacheInvalidator.InvalidateVolunteerAsync(volunteerId, cancellationToken);

        logger.LogInformation("Volunteer {VolunteerId} social networks were updated", volunteerId);

        return UnitResult.Success<Error>();
    }
}
