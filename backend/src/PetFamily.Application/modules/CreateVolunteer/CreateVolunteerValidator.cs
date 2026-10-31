using FluentValidation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.modules.CreateVolunteer;

public class CreateVolunteerValidator : AbstractValidator<CreateVolunteerRequest>
{
    public CreateVolunteerValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(Constants.MaxLowTextLength);

        RuleFor(request => request.Patronymic)
            .MaximumLength(Constants.MaxLowTextLength)
            .When(request => request.Patronymic is not null);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Constants.MaxEmailLength);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MinimumLength(Constants.MinDescriptionLength)
            .MaximumLength(Constants.MaxHighDescription);

        RuleFor(request => request.YearsOfExperience)
            .InclusiveBetween(0, Constants.MaxYearsOfExperience);

        RuleFor(request => request.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9\s\-\(\)]{5,20}$")
            .WithMessage("Phone number has an invalid format.");

        RuleForEach(request => request.SocialLinks)
            .ChildRules(socialLink =>
            {
                socialLink.RuleFor(link => link.Name)
                    .NotEmpty()
                    .MaximumLength(Constants.MaxLowTextLength);

                socialLink.RuleFor(link => link.Url)
                    .NotEmpty()
                    .MaximumLength(Constants.MaxUrlLength)
                    .Must(BeValidUrl)
                    .WithMessage("Social link must be a valid absolute http/https URL.");
            });

        RuleForEach(request => request.DonationsRequisites)
            .ChildRules(donation =>
            {
                donation.RuleFor(details => details.Name)
                    .NotEmpty()
                    .MaximumLength(Constants.MaxLowTextLength);

                donation.RuleFor(details => details.Description)
                    .NotEmpty()
                    .MaximumLength(Constants.MaxMediumTextLength);
            });
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
