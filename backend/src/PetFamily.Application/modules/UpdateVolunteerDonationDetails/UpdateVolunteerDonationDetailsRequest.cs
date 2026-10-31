using FluentValidation;
using PetFamily.Application.Dtos;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.modules.UpdateVolunteerDonationDetails;

public record UpdateVolunteerDonationDetailsRequest(IEnumerable<DonationsRequisites>? DonationDetails);

public class UpdateVolunteerDonationDetailsValidator : AbstractValidator<UpdateVolunteerDonationDetailsRequest>
{
    public UpdateVolunteerDonationDetailsValidator()
    {
        RuleForEach(request => request.DonationDetails)
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
}
