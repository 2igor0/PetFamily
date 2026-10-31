namespace PetFamily.Application.Dtos;

public sealed class VolunteerDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Email { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int PetsFoundHomeCount { get; set; }

    public int PetsSeekingHomeCount { get; set; }

    public int PetsUnderTreatmentCount { get; set; }
}

public sealed class VolunteerDetailsDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Email { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int PetsFoundHomeCount { get; set; }

    public int PetsSeekingHomeCount { get; set; }

    public int PetsUnderTreatmentCount { get; set; }

    public IReadOnlyList<SocialLinkDto> SocialLinks { get; set; } = [];

    public IReadOnlyList<DonationsRequisites> DonationDetails { get; set; } = [];
}

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
