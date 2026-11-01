using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Application.modules.DeletePet;

public class DeletePetHandler(
    IVolunteerRepository repository,
    ILogger<DeletePetHandler> logger)
{
    public async Task<UnitResult<Error>> Execute(
        Guid volunteerId,
        Guid petId,
        CancellationToken cancellationToken = default)
    {
        if (volunteerId == Guid.Empty)
            return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        if (petId == Guid.Empty)
            return Error.Validation("pet.id.is.invalid", "Pet id cannot be empty.");

        var volunteer = await repository.GetById(
            IdVolunteer.Create(volunteerId),
            includePets: true,
            cancellationToken: cancellationToken);

        if (volunteer is null)
            return Error.NotFound("volunteer.not.found", "Volunteer was not found.");

        var pet = volunteer.Pets.FirstOrDefault(existingPet => existingPet.Id == IdPet.Create(petId));
        if (pet is null)
            return Error.NotFound("pet.not.found", "Pet was not found.");

        var deleteResult = pet.Delete();
        if (deleteResult.IsFailure)
            return deleteResult.Error;

        await repository.SaveChanges(cancellationToken);

        logger.LogInformation(
            "Pet {PetId} was soft deleted from volunteer {VolunteerId}",
            petId,
            volunteerId);

        return UnitResult.Success<Error>();
    }
}
