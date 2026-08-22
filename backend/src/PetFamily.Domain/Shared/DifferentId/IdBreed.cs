namespace PetFamily.Domain.Shared.DifferentId;


public record IdBreed
{
    private IdBreed(Guid id)
    {
        Value = id;
    }
    
    public Guid Value { get; }

    public static IdBreed CreateNew() => new(Guid.NewGuid());
    
    public static IdBreed Empty() => new(Guid.Empty);
    
    public static IdBreed Create(Guid id) => new(id); //?? зачем
}