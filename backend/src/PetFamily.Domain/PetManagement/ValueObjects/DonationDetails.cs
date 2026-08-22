using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record DonationDetails
{
    private DonationDetails(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }
    
    public string? Name { get; }
    
    public string? Description { get; }

    public static Result<DonationDetails, Error> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("name.is.invalid", "name is null or empty");
                
        if (string.IsNullOrWhiteSpace(description))
            return Error.Validation("description.is.invalid", "description is null or empty");
                
        return new DonationDetails(name, description);
    }
}