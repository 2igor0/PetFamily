using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Caching;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Application.modules.DeleteVolunteer;

public class DeleteVolunteerHandler(
    IVolunteerRepository repository,
    ICacheInvalidator cacheInvalidator,
    ILogger<DeleteVolunteerHandler> logger)
{
    public async Task<UnitResult<Error>> Execute(
        Guid volunteerId,
        CancellationToken cancellationToken = default)
    {
        if (volunteerId == Guid.Empty)
            return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        var volunteer = await repository.GetById(
            IdVolunteer.Create(volunteerId),
            includePets: true,
            cancellationToken: cancellationToken);

        if (volunteer is null)
            return Error.NotFound("volunteer.not.found", "Volunteer was not found.");

        var deleteResult = volunteer.Delete();
        if (deleteResult.IsFailure)
            return deleteResult.Error;

        await repository.SaveChanges(cancellationToken);
        await cacheInvalidator.InvalidateVolunteerAsync(volunteerId, cancellationToken);

        logger.LogInformation("Volunteer {VolunteerId} was soft deleted", volunteerId);

        return UnitResult.Success<Error>();
    }
}
