using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteerManagement.ValueObjects;

public record Experience
{
    public int Value { get; }

    private Experience()
    {
    }

    private Experience(int value)
    {
        Value = value;
    }

    public static Result<Experience, Error> Create(int years)
    {
        if (years < 0 || years > Constants.MaxYearsOfExperience)
            return Error.Validation(
                "experience.is.invalid",
                $"Experience must be between 0 and {Constants.MaxYearsOfExperience} years.");

        return new Experience(years);
    }
}
