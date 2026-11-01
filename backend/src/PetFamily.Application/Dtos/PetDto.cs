namespace PetFamily.Application.Dtos;

public class PetDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid SpeciesId { get; set; }

    public string SpeciesName { get; set; } = null!;

    public Guid BreedId { get; set; }

    public string BreedName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string HealthInfoDescription { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public int HouseNumber { get; set; }

    public int Weight { get; set; }

    public int Height { get; set; }

    public string OwnerPhoneNumber { get; set; } = null!;

    public bool IsNeutered { get; set; }

    public DateOnly BirthDate { get; set; }

    public int Age { get; set; }

    public bool IsVaccinated { get; set; }

    public string HelpStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid VolunteerId { get; set; }

    public string VolunteerFirstName { get; set; } = null!;

    public string VolunteerLastName { get; set; } = null!;

    public IReadOnlyList<PetPhotoDto> Photos { get; set; } = [];
}

public sealed class PetDetailsDto : PetDto
{
    public IReadOnlyList<DonationsRequisites> DonationDetails { get; set; } = [];
}

public sealed class PetPhotoDto
{
    public string Path { get; set; } = null!;

    public bool IsMain { get; set; }

    public string? Url { get; set; }
}
