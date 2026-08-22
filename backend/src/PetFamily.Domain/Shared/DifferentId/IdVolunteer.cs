namespace PetFamily.Domain.Shared.DifferentId;

public record IdVolunteer
{
    private IdVolunteer(Guid id)
    {
        Value = id;
    }
    
    public Guid Value { get; }

    public static IdVolunteer CreateNewId() => new(Guid.NewGuid());
    
    public static IdVolunteer Empty() => new(Guid.Empty);
    
    public static IdVolunteer Create(Guid id) => new(id); //?? зачем
}