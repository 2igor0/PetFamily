namespace PetFamily.Domain.Shared.DifferentId;

public record IdPet
{
    private IdPet(Guid id)
    {
        Value = id;
    }
    
    public Guid Value { get; }

    public static IdPet CreateNew() => new(Guid.NewGuid());
    
    public static IdPet Empty() => new(Guid.Empty);
    
    public static IdPet Create(Guid id) => new(id); //?? зачем
}