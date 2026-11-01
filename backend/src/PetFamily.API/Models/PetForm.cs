namespace PetFamily.API.Models;

public sealed class PetForm
{
    public string Name { get; set; } = null!;

    public Guid SpeciesId { get; set; }

    public Guid BreedId { get; set; }

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

    public bool IsVaccinated { get; set; }

    public string HelpStatus { get; set; } = null!;

    public string? DonationDetailsJson { get; set; }

    public List<IFormFile>? Photos { get; set; }
}
