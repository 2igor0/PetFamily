namespace PetFamily.Domain.Shared;

public abstract class Entity<TId> // может другие айдишники добавть
{
    protected Entity(TId id)
    {
        Id = id;
    }
    
    public TId Id { get; private set; }
}
