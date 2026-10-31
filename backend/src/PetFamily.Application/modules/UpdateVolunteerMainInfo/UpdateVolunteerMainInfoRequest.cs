using FluentValidation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.modules.UpdateVolunteerMainInfo;

public record UpdateVolunteerMainInfoRequest(
    string FirstName,
    string LastName,
    string? Patronymic,
    string Email,
    string Description,
    int YearsOfExperience,
    string PhoneNumber);

public class UpdateVolunteerMainInfoValidator : AbstractValidator<UpdateVolunteerMainInfoRequest>
{
    public UpdateVolunteerMainInfoValidator()
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
    }
}
