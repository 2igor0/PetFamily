using FluentValidation;

namespace PetFamily.Application.modules.GetVolunteers;

public record GetVolunteersRequest(int Page = 1, int PageSize = 10);

public class GetVolunteersValidator : AbstractValidator<GetVolunteersRequest>
{
    public GetVolunteersValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);
    }
}
