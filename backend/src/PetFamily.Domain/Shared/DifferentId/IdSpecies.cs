namespace PetFamily.Domain.Shared.DifferentId;

public record IdSpecies
{
    private IdSpecies(Guid id)
    {
        Value = id;
    }
    
    public Guid Value { get; }

    public static IdSpecies CreateNewId() => new(Guid.NewGuid());
    
    public static IdSpecies Empty() => new(Guid.Empty);
    
    public static IdSpecies Create(Guid id) => new(id); //?? зачем
}