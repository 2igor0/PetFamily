using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.Domain.VolunteerManagement;

public class Volunteer : Shared.Entity<IdVolunteer>
{
    private readonly List<SocialNetwork> _socialNetworks = [];

    private readonly List<DonationDetails> _donationDetails = [];

    private readonly List<Pet> _pets = [];

    // EF Core
    private Volunteer() : base(IdVolunteer.Empty())
    {
        FullName = null!;
        Email = null!;
        Description = null!;
        YearsOfExperience = null!;
        OwnerPhoneNumber = null!;
    }

    private Volunteer(
        IdVolunteer id,
        FullName fullName,
        Email email,
        Description description,
        Experience yearsOfExperience,
        PhoneNumber ownerPhoneNumber,
        IEnumerable<SocialNetwork>? socialNetworks,
        IEnumerable<DonationDetails>? donationDetails) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        YearsOfExperience = yearsOfExperience;
        OwnerPhoneNumber = ownerPhoneNumber;
        _socialNetworks = socialNetworks?.ToList() ?? [];
        _donationDetails = donationDetails?.ToList() ?? [];
        CreatedAt = DateTime.UtcNow;
    }

    public FullName FullName { get; private set; }

    public Email Email { get; private set; }

    public Description Description { get; private set; }

    public Experience YearsOfExperience { get; private set; }

    public PhoneNumber OwnerPhoneNumber { get; private set; }

    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;

    public IReadOnlyList<DonationDetails> DonationDetails => _donationDetails;

    public IReadOnlyList<Pet> Pets => _pets;

    public DateTime CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public static Result<Volunteer, Error> Create(
        IdVolunteer id,
        string? firstName,
        string? lastName,
        string? patronymic,
        string? email,
        string? description,
        int yearsOfExperience,
        string? ownerPhoneNumber,
        IEnumerable<SocialNetwork>? socialNetworks = null,
        IEnumerable<DonationDetails>? donationDetails = null)
    {
        if (id.Value == Guid.Empty)
            return Error.Validation("volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        var fullNameResult = FullName.Create(firstName, lastName, patronymic);
        if (fullNameResult.IsFailure)
            return fullNameResult.Error;

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var experienceResult = Experience.Create(yearsOfExperience);
        if (experienceResult.IsFailure)
            return experienceResult.Error;

        var phoneNumberResult = PhoneNumber.Create(ownerPhoneNumber);
        if (phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;

        var socialNetworksList = socialNetworks?.ToList() ?? [];
        if (socialNetworksList.Any(network => network is null))
            return Error.Validation("volunteer.social.networks.is.invalid", "Social networks cannot contain null.");

        var donationDetailsList = donationDetails?.ToList() ?? [];
        if (donationDetailsList.Any(details => details is null))
            return Error.Validation(
                "volunteer.donation.details.is.invalid",
                "Donation details cannot contain null.");

        var volunteer = new Volunteer(
            id,
            fullNameResult.Value,
            emailResult.Value,
            descriptionResult.Value,
            experienceResult.Value,
            phoneNumberResult.Value,
            socialNetworksList,
            donationDetailsList);

        return volunteer;
    }

    public UnitResult<Error> UpdateMainInfo(
        FullName? fullName,
        Email? email,
        Description? description,
        Experience? yearsOfExperience,
        PhoneNumber? ownerPhoneNumber)
    {
        if (IsDeleted)
            return Error.Validation("volunteer.is.deleted", "Cannot update a deleted volunteer.");

        if (fullName is null)
            return Error.Validation("full.name.is.invalid", "Full name cannot be null.");

        if (email is null)
            return Error.Validation("email.is.invalid", "Email cannot be null.");

        if (description is null)
            return Error.Validation("description.is.invalid", "Description cannot be null.");

        if (yearsOfExperience is null)
            return Error.Validation("experience.is.invalid", "Experience cannot be null.");

        if (ownerPhoneNumber is null)
            return Error.Validation("phone.number.is.invalid", "Phone number cannot be null.");

        FullName = fullName;
        Email = email;
        Description = description;
        YearsOfExperience = yearsOfExperience;
        OwnerPhoneNumber = ownerPhoneNumber;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateSocialNetworks(IEnumerable<SocialNetwork>? socialNetworks)
    {
        if (IsDeleted)
            return Error.Validation("volunteer.is.deleted", "Cannot update a deleted volunteer.");

        var socialNetworksList = socialNetworks?.ToList() ?? [];
        if (socialNetworksList.Any(network => network is null))
            return Error.Validation("volunteer.social.networks.is.invalid", "Social networks cannot contain null.");

        _socialNetworks.Clear();
        _socialNetworks.AddRange(socialNetworksList);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateDonationDetails(IEnumerable<DonationDetails>? donationDetails)
    {
        if (IsDeleted)
            return Error.Validation("volunteer.is.deleted", "Cannot update a deleted volunteer.");

        var donationDetailsList = donationDetails?.ToList() ?? [];
        if (donationDetailsList.Any(details => details is null))
            return Error.Validation(
                "volunteer.donation.details.is.invalid",
                "Donation details cannot contain null.");

        _donationDetails.Clear();
        _donationDetails.AddRange(donationDetailsList);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddPet(Pet? pet)
    {
        if (IsDeleted)
            return Error.Validation("volunteer.is.deleted", "Cannot add a pet to a deleted volunteer.");

        if (pet is null)
            return Error.Validation("pet.is.invalid", "Pet cannot be null.");

        if (pet.VolunteerId != Id)
            return Error.Validation(
                "pet.volunteer.id.is.invalid",
                "Pet belongs to another volunteer.");

        if (_pets.Any(existingPet => existingPet.Id == pet.Id))
            return Error.Conflict("pet.already.exists", "Pet already belongs to this volunteer.");

        _pets.Add(pet);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DeletePet(IdPet? petId)
    {
        if (petId is null)
            return Error.Validation("pet.id.is.invalid", "Pet id cannot be null.");

        var pet = _pets.FirstOrDefault(existingPet => existingPet.Id == petId);
        if (pet is null)
            return Error.NotFound("pet.not.found", "Pet was not found.");

        return pet.Delete();
    }

    public int GetAdoptedPetsCount() =>
        _pets.Count(pet => !pet.IsDeleted && pet.HelpStatus == PetManagement.Enums.HelpStatus.FoundHome);

    public int GetPetsSeekingHomeCount() =>
        _pets.Count(pet => !pet.IsDeleted && pet.HelpStatus == PetManagement.Enums.HelpStatus.LookingForHome);

    public int GetPetsUnderTreatmentCount() =>
        _pets.Count(pet => !pet.IsDeleted && pet.HelpStatus == PetManagement.Enums.HelpStatus.NeedsHelp);

    public UnitResult<Error> Delete()
    {
        if (IsDeleted)
            return UnitResult.Success<Error>();

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        foreach (var pet in _pets)
            pet.Delete();

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Restore()
    {
        IsDeleted = false;
        DeletedAt = null;

        foreach (var pet in _pets)
            pet.Restore();

        return UnitResult.Success<Error>();
    }
}
