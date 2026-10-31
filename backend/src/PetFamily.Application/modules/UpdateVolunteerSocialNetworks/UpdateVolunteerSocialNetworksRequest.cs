using FluentValidation;
using PetFamily.Application.Dtos;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.modules.UpdateVolunteerSocialNetworks;

public record UpdateVolunteerSocialNetworksRequest(IEnumerable<SocialLinkDto>? SocialLinks);

public class UpdateVolunteerSocialNetworksValidator : AbstractValidator<UpdateVolunteerSocialNetworksRequest>
{
    public UpdateVolunteerSocialNetworksValidator()
    {
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
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
