using System.Text.Json;
using FluentValidation;
using PetFamily.API.Models;
using PetFamily.Application.Dtos;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Validators;

public sealed class PetFormValidator : AbstractValidator<PetForm>
{
    private const int MaxPhotosCount = 10;
    private const long MaxPhotoSizeBytes = 10 * 1024 * 1024;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PetFormValidator()
    {
        RuleFor(form => form.Name)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(form => form.SpeciesId)
            .NotEmpty();

        RuleFor(form => form.BreedId)
            .NotEmpty();

        RuleFor(form => form.Description)
            .NotEmpty()
            .MaximumLength(Constants.MaxHighDescription);

        RuleFor(form => form.Color)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(form => form.HealthInfoDescription)
            .NotEmpty()
            .MaximumLength(Constants.MaxHighDescription);

        RuleFor(form => form.Country)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(form => form.City)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(form => form.Street)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(form => form.HouseNumber)
            .GreaterThan(0);

        RuleFor(form => form.Weight)
            .InclusiveBetween(0, Constants.MaxPhysicalAttribute);

        RuleFor(form => form.Height)
            .InclusiveBetween(0, Constants.MaxPhysicalAttribute);

        RuleFor(form => form.OwnerPhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9\s\-\(\)]{5,20}$")
            .WithMessage("Phone number has an invalid format.");

        RuleFor(form => form.BirthDate)
            .Must(birthDate => birthDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Birth date cannot be in the future.");

        RuleFor(form => form.HelpStatus)
            .Must(status => Enum.TryParse<HelpStatus>(status, true, out _))
            .WithMessage("Help status is invalid.");

        RuleFor(form => form.DonationDetailsJson)
            .Must(BeValidDonationDetailsJson)
            .When(form => !string.IsNullOrWhiteSpace(form.DonationDetailsJson))
            .WithMessage("Donation details JSON is invalid.");

        RuleFor(form => form.Photos)
            .Must(photos => photos is null || photos.Count <= MaxPhotosCount)
            .WithMessage($"A pet cannot have more than {MaxPhotosCount} photos.");

        RuleForEach(form => form.Photos)
            .Must(BeValidPhoto)
            .WithMessage("Each photo must be a non-empty image up to 10 MB.");
    }

    private static bool BeValidDonationDetailsJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return true;

        try
        {
            var details = JsonSerializer.Deserialize<List<DonationsRequisites>>(json, JsonOptions);
            if (details is null)
                return true;

            return details.All(detailsItem =>
                !string.IsNullOrWhiteSpace(detailsItem.Name)
                && detailsItem.Name.Length <= Constants.MaxLowTextLength
                && !string.IsNullOrWhiteSpace(detailsItem.Description)
                && detailsItem.Description.Length <= Constants.MaxMediumTextLength);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool BeValidPhoto(IFormFile file)
    {
        return file.Length > 0
               && file.Length <= MaxPhotoSizeBytes
               && file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }
}
