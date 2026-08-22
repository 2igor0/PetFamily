using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteerManagement.ValueObjects;

public record FullName 
{
    public string FirstName { get; }
    
    public string LastName { get; }
    
    public string? Patronymic { get; }

    private FullName(string firstName, string lastName, string? patronymic)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }

    public static Result<FullName, Error> Create(string firstName, string lastName, string? patronymic)
    {
        // валидация

        return new FullName(firstName, lastName, patronymic);
    }
}