using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Infrastructure.Converters;

public sealed class IdVolunteerConverter : ValueConverter<IdVolunteer, Guid>
{
    public IdVolunteerConverter()
        : base(id => id.Value, value => IdVolunteer.Create(value))
    {
    }
}

public sealed class IdPetConverter : ValueConverter<IdPet, Guid>
{
    public IdPetConverter()
        : base(id => id.Value, value => IdPet.Create(value))
    {
    }
}

public sealed class IdSpeciesConverter : ValueConverter<IdSpecies, Guid>
{
    public IdSpeciesConverter()
        : base(id => id.Value, value => IdSpecies.Create(value))
    {
    }
}

public sealed class IdBreedConverter : ValueConverter<IdBreed, Guid>
{
    public IdBreedConverter()
        : base(id => id.Value, value => IdBreed.Create(value))
    {
    }
}
