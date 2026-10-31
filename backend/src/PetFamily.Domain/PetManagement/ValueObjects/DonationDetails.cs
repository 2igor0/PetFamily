using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record DonationDetails
{
    public string Name { get; }

    public string Description { get; }

    private DonationDetails()
    {
        Name = null!;
        Description = null!;
    }

    private DonationDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Result<DonationDetails, Error> Create(string? name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("donation.name.is.invalid", "Donation name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            return Error.Validation(
                "donation.description.is.invalid",
                "Donation description cannot be empty.");

        var trimmedName = name.Trim();
        var trimmedDescription = description.Trim();

        if (trimmedName.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "donation.name.is.too.long",
                $"Donation name cannot exceed {Constants.MaxLowTextLength} characters.");

        if (trimmedDescription.Length > Constants.MaxMediumTextLength)
            return Error.Validation(
                "donation.description.is.too.long",
                $"Donation description cannot exceed {Constants.MaxMediumTextLength} characters.");

        return new DonationDetails(trimmedName, trimmedDescription);
    }
}
