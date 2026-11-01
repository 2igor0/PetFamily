using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Models;
using PetFamily.Application.Dtos;
using PetFamily.Application.FileStorage;
using PetFamily.Application.modules.CreatePet;
using PetFamily.Application.modules.DeletePet;
using PetFamily.Application.modules.UpdatePet;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("volunteers/{volunteerId:guid}/pets")]
public class VolunteerPetsController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<Guid>> Create(
        Guid volunteerId,
        [FromForm] PetForm form,
        [FromServices] CreatePetHandler handler,
        CancellationToken cancellationToken)
    {
        var requestResult = ToCreateRequest(form);
        if (requestResult.IsFailure)
            return requestResult.Error.ToActionResult();

        var result = await handler.Execute(volunteerId, requestResult.Value, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{petId:guid}")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult> Update(
        Guid volunteerId,
        Guid petId,
        [FromForm] PetForm form,
        [FromServices] UpdatePetHandler handler,
        CancellationToken cancellationToken)
    {
        var requestResult = ToUpdateRequest(form);
        if (requestResult.IsFailure)
            return requestResult.Error.ToActionResult();

        var result = await handler.Execute(volunteerId, petId, requestResult.Value, cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{petId:guid}")]
    public async Task<ActionResult> Delete(
        Guid volunteerId,
        Guid petId,
        [FromServices] DeletePetHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(volunteerId, petId, cancellationToken);

        return result.ToActionResult();
    }

    private static Result<CreatePetRequest, Error> ToCreateRequest(PetForm form)
    {
        var donationDetailsResult = ParseDonationDetails(form.DonationDetailsJson);
        if (donationDetailsResult.IsFailure)
            return donationDetailsResult.Error;

        return new CreatePetRequest(
            form.Name,
            form.SpeciesId,
            form.BreedId,
            form.Description,
            form.Color,
            form.HealthInfoDescription,
            form.Country,
            form.City,
            form.Street,
            form.HouseNumber,
            form.Weight,
            form.Height,
            form.OwnerPhoneNumber,
            form.IsNeutered,
            form.BirthDate,
            form.IsVaccinated,
            form.HelpStatus,
            donationDetailsResult.Value,
            MapPhotos(form.Photos));
    }

    private static Result<UpdatePetRequest, Error> ToUpdateRequest(PetForm form)
    {
        var donationDetailsResult = ParseDonationDetails(form.DonationDetailsJson);
        if (donationDetailsResult.IsFailure)
            return donationDetailsResult.Error;

        return new UpdatePetRequest(
            form.Name,
            form.SpeciesId,
            form.BreedId,
            form.Description,
            form.Color,
            form.HealthInfoDescription,
            form.Country,
            form.City,
            form.Street,
            form.HouseNumber,
            form.Weight,
            form.Height,
            form.OwnerPhoneNumber,
            form.IsNeutered,
            form.BirthDate,
            form.IsVaccinated,
            form.HelpStatus,
            donationDetailsResult.Value,
            MapPhotos(form.Photos));
    }

    private static Result<IReadOnlyList<DonationsRequisites>, Error> ParseDonationDetails(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return Array.Empty<DonationsRequisites>();

        try
        {
            var donationDetails = JsonSerializer.Deserialize<List<DonationsRequisites>>(json, JsonOptions)
                                  ?? [];

            return donationDetails;
        }
        catch (JsonException)
        {
            return Error.Validation(
                "donation.details.is.invalid",
                "Donation details JSON is invalid.");
        }
    }

    private static List<UploadFileDto> MapPhotos(IEnumerable<IFormFile>? photos)
    {
        return photos?
            .Select(file => new UploadFileDto(
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length))
            .ToList() ?? [];
    }
}
