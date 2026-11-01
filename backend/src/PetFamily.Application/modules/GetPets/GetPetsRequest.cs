using FluentValidation;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.modules.GetPets;

public record GetPetsRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    string? Species = null,
    string? Breed = null,
    string? Color = null,
    string? City = null,
    string? Country = null,
    int? AgeFrom = null,
    int? AgeTo = null,
    string? HelpStatus = null,
    Guid? VolunteerId = null,
    string? SortBy = "CreatedAt",
    string? SortDirection = "Desc");

public class GetPetsValidator : AbstractValidator<GetPetsRequest>
{
    private static readonly string[] AllowedSortFields =
    [
        "Name",
        "Age",
        "Breed",
        "Color",
        "Species",
        "City",
        "Country",
        "Volunteer",
        "CreatedAt"
    ];

    public GetPetsValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.Name)
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.Species)
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.Breed)
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.Color)
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.City)
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.Country)
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.AgeFrom)
            .InclusiveBetween(0, 100)
            .When(request => request.AgeFrom.HasValue);

        RuleFor(request => request.AgeTo)
            .InclusiveBetween(0, 100)
            .When(request => request.AgeTo.HasValue);

        RuleFor(request => request)
            .Must(request => !request.AgeFrom.HasValue
                             || !request.AgeTo.HasValue
                             || request.AgeFrom <= request.AgeTo)
            .WithMessage("AgeFrom cannot be greater than AgeTo.");

        RuleFor(request => request.HelpStatus)
            .Must(status => Enum.TryParse<HelpStatus>(status, true, out _))
            .When(request => !string.IsNullOrWhiteSpace(request.HelpStatus))
            .WithMessage("Help status is invalid.");

        RuleFor(request => request.VolunteerId)
            .NotEqual(Guid.Empty)
            .When(request => request.VolunteerId.HasValue);

        RuleFor(request => request.SortBy)
            .Must(sortBy => AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .When(request => !string.IsNullOrWhiteSpace(request.SortBy))
            .WithMessage("Sort field is invalid.");

        RuleFor(request => request.SortDirection)
            .Must(direction => direction is not null
                               && (direction.Equals("Asc", StringComparison.OrdinalIgnoreCase)
                                   || direction.Equals("Desc", StringComparison.OrdinalIgnoreCase)))
            .When(request => !string.IsNullOrWhiteSpace(request.SortDirection))
            .WithMessage("Sort direction must be 'Asc' or 'Desc'.");
    }
}
