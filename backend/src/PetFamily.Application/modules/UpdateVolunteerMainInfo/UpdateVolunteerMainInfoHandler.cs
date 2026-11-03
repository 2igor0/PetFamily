using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Caching;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.Application.modules.UpdateVolunteerMainInfo;

public class UpdateVolunteerMainInfoHandler(
    IVolunteerRepository repository,
    ICacheInvalidator cacheInvalidator,
    ILogger<UpdateVolunteerMainInfoHandler> logger)
{
    public async Task<UnitResult<Error>> Execute(
        Guid volunteerId,
        UpdateVolunteerMainInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (volunteerId == Guid.Empty)
            return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        var volunteer = await repository.GetById(
            IdVolunteer.Create(volunteerId),
            cancellationToken: cancellationToken);

        if (volunteer is null)
            return Error.NotFound("volunteer.not.found", "Volunteer was not found.");

        var fullNameResult = FullName.Create(request.FirstName, request.LastName, request.Patronymic);
        if (fullNameResult.IsFailure)
            return fullNameResult.Error;

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var descriptionResult = Description.Create(request.Description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var experienceResult = Experience.Create(request.YearsOfExperience);
        if (experienceResult.IsFailure)
            return experienceResult.Error;

        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;

        var updateResult = volunteer.UpdateMainInfo(
            fullNameResult.Value,
            emailResult.Value,
            descriptionResult.Value,
            experienceResult.Value,
            phoneNumberResult.Value);

        if (updateResult.IsFailure)
            return updateResult.Error;

        await repository.SaveChanges(cancellationToken);
        await cacheInvalidator.InvalidateVolunteerAsync(volunteerId, cancellationToken);

        logger.LogInformation("Volunteer {VolunteerId} main info was updated", volunteerId);

        return UnitResult.Success<Error>();
    }
}
