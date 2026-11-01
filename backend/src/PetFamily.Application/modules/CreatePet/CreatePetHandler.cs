using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.FileStorage;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Application.modules.CreatePet;

public class CreatePetHandler(
    IVolunteerRepository repository,
    IFileStorage fileStorage,
    ILogger<CreatePetHandler> logger)
{
    private const int MaxPhotosCount = 10;

    public async Task<Result<Guid, Error>> Execute(
        Guid volunteerId,
        CreatePetRequest request,
        CancellationToken cancellationToken = default)
    {
        var files = request.Photos?.ToList() ?? [];
        var uploadedPaths = new List<string>();

        try
        {
            if (volunteerId == Guid.Empty)
                return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

            if (!Enum.TryParse<HelpStatus>(request.HelpStatus, true, out var helpStatus))
                return Error.Validation("pet.help.status.is.invalid", "Help status is invalid.");

            if (files.Count > MaxPhotosCount)
                return Error.Validation(
                    "pet.photos.too.many",
                    $"A pet cannot have more than {MaxPhotosCount} photos.");

            var volunteer = await repository.GetById(
                IdVolunteer.Create(volunteerId),
                includePets: true,
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

            var petResult = Pet.Create(
                IdVolunteer.Create(volunteerId),
                request.Name,
                request.SpeciesId,
                request.BreedId,
                request.Description,
                request.Color,
                request.HealthInfoDescription,
                request.Country,
                request.City,
                request.Street,
                request.HouseNumber,
                request.Weight,
                request.Height,
                request.OwnerPhoneNumber,
                request.IsNeutered,
                request.BirthDate,
                request.IsVaccinated,
                helpStatus,
                donationDetails);

            if (petResult.IsFailure)
                return petResult.Error;

            var pet = petResult.Value;
            var photos = new List<PetPhoto>();

            for (var index = 0; index < files.Count; index++)
            {
                var file = files[index];
                var fileName = BuildFileName(pet.Id.Value, file.FileName);

                await using var stream = file.Content;

                var uploadResult = await fileStorage.UploadFile(
                    stream,
                    FileStorageBuckets.Pets,
                    fileName,
                    file.ContentType,
                    cancellationToken);

                if (uploadResult.IsFailure)
                {
                    await CleanupFiles(uploadedPaths);
                    return uploadResult.Error;
                }

                uploadedPaths.Add(uploadResult.Value);

                var photoResult = PetPhoto.Create(uploadResult.Value, index == 0);
                if (photoResult.IsFailure)
                {
                    await CleanupFiles(uploadedPaths);
                    return photoResult.Error;
                }

                photos.Add(photoResult.Value);
            }

            var replacePhotosResult = pet.ReplacePhotos(photos);
            if (replacePhotosResult.IsFailure)
            {
                await CleanupFiles(uploadedPaths);
                return replacePhotosResult.Error;
            }

            var addPetResult = volunteer.AddPet(pet);
            if (addPetResult.IsFailure)
            {
                await CleanupFiles(uploadedPaths);
                return addPetResult.Error;
            }

            await repository.SaveChanges(cancellationToken);

            logger.LogInformation(
                "Pet {PetId} was created for volunteer {VolunteerId}",
                pet.Id.Value,
                volunteerId);

            return pet.Id.Value;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create pet for volunteer {VolunteerId}", volunteerId);
            await CleanupFiles(uploadedPaths);
            return Error.Failure("pet.create.failed", "Failed to create pet.");
        }
        finally
        {
            foreach (var file in files)
                await file.Content.DisposeAsync();
        }
    }

    private async Task CleanupFiles(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var deleteResult = await fileStorage.DeleteFile(
                FileStorageBuckets.Pets,
                fileName,
                CancellationToken.None);

            if (deleteResult.IsFailure)
            {
                logger.LogWarning(
                    "Failed to delete uploaded file {FileName} after pet creation failure",
                    fileName);
            }
        }
    }

    private static string BuildFileName(Guid petId, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);

        return $"{petId}/{Guid.NewGuid():N}{extension}";
    }
}
