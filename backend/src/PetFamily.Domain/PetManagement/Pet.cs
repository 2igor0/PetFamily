using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.Domain.PetManagement;

public class Pet : Shared.Entity<IdPet>
{
    private readonly List<PetPhoto> _photos = [];

    private readonly List<DonationDetails> _donationDetails = [];

    // EF Core
    private Pet() : base(IdPet.Empty())
    {
        VolunteerId = IdVolunteer.Empty();
        Name = null!;
        SpeciesBreed = null!;
        Description = null!;
        Color = null!;
        HealthInfoDescription = null!;
        CurrentAddress = null!;
        Weight = null!;
        Height = null!;
        OwnerPhoneNumber = null!;
    }

    private Pet(
        IdPet id,
        IdVolunteer volunteerId,
        Title name,
        SpeciesBreed speciesBreed,
        Description description,
        Color color,
        Description healthInfoDescription,
        Address currentAddress,
        PhysicalAttributes weight,
        PhysicalAttributes height,
        PhoneNumber ownerPhoneNumber,
        bool isNeutered,
        DateOnly birthDate,
        bool isVaccinated,
        HelpStatus helpStatus,
        IEnumerable<DonationDetails>? donationDetails,
        IEnumerable<PetPhoto>? photos) : base(id)
    {
        VolunteerId = volunteerId;
        Name = name;
        SpeciesBreed = speciesBreed;
        Description = description;
        Color = color;
        HealthInfoDescription = healthInfoDescription;
        CurrentAddress = currentAddress;
        Weight = weight;
        Height = height;
        OwnerPhoneNumber = ownerPhoneNumber;
        IsNeutered = isNeutered;
        BirthDate = birthDate;
        IsVaccinated = isVaccinated;
        HelpStatus = helpStatus;
        CreatedAt = DateTime.UtcNow;
        _donationDetails = donationDetails?.ToList() ?? [];
        _photos = photos?.ToList() ?? [];
        EnsureMainPhoto();
    }

    public IdVolunteer VolunteerId { get; private set; }

    public Title Name { get; private set; }

    public SpeciesBreed SpeciesBreed { get; private set; }

    public Description Description { get; private set; }

    public Color Color { get; private set; }

    public Description HealthInfoDescription { get; private set; }

    public Address CurrentAddress { get; private set; }

    public PhysicalAttributes Weight { get; private set; }

    public PhysicalAttributes Height { get; private set; }

    public PhoneNumber OwnerPhoneNumber { get; private set; }

    public bool IsNeutered { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public bool IsVaccinated { get; private set; }

    public HelpStatus HelpStatus { get; private set; }

    public IReadOnlyList<DonationDetails> DonationDetails => _donationDetails;

    public IReadOnlyList<PetPhoto> Photos => _photos;

    public DateTime CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public static Result<Pet, Error> Create(
        IdVolunteer volunteerId,
        string? name,
        Guid speciesId,
        Guid breedId,
        string? description,
        string? color,
        string? healthInfoDescription,
        string? country,
        string? city,
        string? street,
        int houseNumber,
        int weight,
        int height,
        string? ownerPhoneNumber,
        bool isNeutered,
        DateOnly birthDate,
        bool isVaccinated,
        HelpStatus helpStatus,
        IEnumerable<DonationDetails>? donationDetails = null,
        IEnumerable<PetPhoto>? photos = null)
    {
        if (volunteerId.Value == Guid.Empty)
            return Error.Validation("pet.volunteer.id.is.invalid", "Volunteer id cannot be empty.");

        if (!Enum.IsDefined(helpStatus))
            return Error.Validation("pet.help.status.is.invalid", "Help status is invalid.");

        if (birthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return Error.Validation("pet.birth.date.is.invalid", "Birth date cannot be in the future.");

        var nameResult = Title.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var speciesBreedResult = SpeciesBreed.Create(
            IdSpecies.Create(speciesId),
            IdBreed.Create(breedId));
        if (speciesBreedResult.IsFailure)
            return speciesBreedResult.Error;

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var colorResult = Color.Create(color);
        if (colorResult.IsFailure)
            return colorResult.Error;

        var healthInfoResult = Description.Create(healthInfoDescription);
        if (healthInfoResult.IsFailure)
            return healthInfoResult.Error;

        var addressResult = Address.Create(country, city, street, houseNumber);
        if (addressResult.IsFailure)
            return addressResult.Error;

        var weightResult = PhysicalAttributes.Create(weight);
        if (weightResult.IsFailure)
            return weightResult.Error;

        var heightResult = PhysicalAttributes.Create(height);
        if (heightResult.IsFailure)
            return heightResult.Error;

        var phoneNumberResult = PhoneNumber.Create(ownerPhoneNumber);
        if (phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;

        var donationDetailsList = donationDetails?.ToList() ?? [];
        if (donationDetailsList.Any(details => details is null))
            return Error.Validation("pet.donation.details.is.invalid", "Donation details cannot contain null.");

        var photosList = photos?.ToList() ?? [];
        if (photosList.Any(photo => photo is null))
            return Error.Validation("pet.photos.is.invalid", "Photos cannot contain null.");

        if (photosList.Count(photo => photo.IsMain) > 1)
            return Error.Validation("pet.photos.main.is.invalid", "Only one photo can be main.");

        var pet = new Pet(
            IdPet.CreateNew(),
            volunteerId,
            nameResult.Value,
            speciesBreedResult.Value,
            descriptionResult.Value,
            colorResult.Value,
            healthInfoResult.Value,
            addressResult.Value,
            weightResult.Value,
            heightResult.Value,
            phoneNumberResult.Value,
            isNeutered,
            birthDate,
            isVaccinated,
            helpStatus,
            donationDetailsList,
            photosList);

        return pet;
    }

    public UnitResult<Error> Update(
        string? name,
        Guid speciesId,
        Guid breedId,
        string? description,
        string? color,
        string? healthInfoDescription,
        string? country,
        string? city,
        string? street,
        int houseNumber,
        int weight,
        int height,
        string? ownerPhoneNumber,
        bool isNeutered,
        DateOnly birthDate,
        bool isVaccinated,
        HelpStatus helpStatus)
    {
        if (IsDeleted)
            return Error.Validation("pet.is.deleted", "Cannot update a deleted pet.");

        if (!Enum.IsDefined(helpStatus))
            return Error.Validation("pet.help.status.is.invalid", "Help status is invalid.");

        if (birthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return Error.Validation("pet.birth.date.is.invalid", "Birth date cannot be in the future.");

        var nameResult = Title.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var speciesBreedResult = SpeciesBreed.Create(
            IdSpecies.Create(speciesId),
            IdBreed.Create(breedId));
        if (speciesBreedResult.IsFailure)
            return speciesBreedResult.Error;

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var colorResult = Color.Create(color);
        if (colorResult.IsFailure)
            return colorResult.Error;

        var healthInfoResult = Description.Create(healthInfoDescription);
        if (healthInfoResult.IsFailure)
            return healthInfoResult.Error;

        var addressResult = Address.Create(country, city, street, houseNumber);
        if (addressResult.IsFailure)
            return addressResult.Error;

        var weightResult = PhysicalAttributes.Create(weight);
        if (weightResult.IsFailure)
            return weightResult.Error;

        var heightResult = PhysicalAttributes.Create(height);
        if (heightResult.IsFailure)
            return heightResult.Error;

        var phoneNumberResult = PhoneNumber.Create(ownerPhoneNumber);
        if (phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;

        Name = nameResult.Value;
        SpeciesBreed = speciesBreedResult.Value;
        Description = descriptionResult.Value;
        Color = colorResult.Value;
        HealthInfoDescription = healthInfoResult.Value;
        CurrentAddress = addressResult.Value;
        Weight = weightResult.Value;
        Height = heightResult.Value;
        OwnerPhoneNumber = phoneNumberResult.Value;
        IsNeutered = isNeutered;
        BirthDate = birthDate;
        IsVaccinated = isVaccinated;
        HelpStatus = helpStatus;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddPhoto(PetPhoto? photo)
    {
        if (photo is null)
            return Error.Validation("pet.photo.is.invalid", "Photo cannot be null.");

        if (_photos.Any(existingPhoto => existingPhoto.Path == photo.Path))
            return Error.Conflict("pet.photo.already.exists", "Photo with this path already exists.");

        if (photo.IsMain)
        {
            for (var index = 0; index < _photos.Count; index++)
                _photos[index] = _photos[index].WithIsMain(false);
        }
        else if (_photos.Count == 0)
        {
            photo = photo.WithIsMain(true);
        }

        _photos.Add(photo);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> RemovePhoto(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Error.Validation("pet.photo.path.is.invalid", "Photo path cannot be empty.");

        var photo = _photos.FirstOrDefault(existingPhoto => existingPhoto.Path == path);
        if (photo is null)
            return Error.NotFound("pet.photo.not.found", "Photo was not found.");

        _photos.Remove(photo);

        if (photo.IsMain && _photos.Count > 0)
            _photos[0] = _photos[0].WithIsMain(true);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> SetMainPhoto(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Error.Validation("pet.photo.path.is.invalid", "Photo path cannot be empty.");

        var photo = _photos.FirstOrDefault(existingPhoto => existingPhoto.Path == path);
        if (photo is null)
            return Error.NotFound("pet.photo.not.found", "Photo was not found.");

        for (var index = 0; index < _photos.Count; index++)
            _photos[index] = _photos[index].WithIsMain(_photos[index].Path == path);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> ReplacePhotos(IEnumerable<PetPhoto>? photos)
    {
        var photosList = photos?.ToList() ?? [];
        if (photosList.Any(photo => photo is null))
            return Error.Validation("pet.photos.is.invalid", "Photos cannot contain null.");

        if (photosList.Count(photo => photo.IsMain) > 1)
            return Error.Validation("pet.photos.main.is.invalid", "Only one photo can be main.");

        _photos.Clear();
        _photos.AddRange(photosList);
        EnsureMainPhoto();

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> ReplaceDonationDetails(IEnumerable<DonationDetails>? donationDetails)
    {
        var donationDetailsList = donationDetails?.ToList() ?? [];
        if (donationDetailsList.Any(details => details is null))
            return Error.Validation(
                "pet.donation.details.is.invalid",
                "Donation details cannot contain null.");

        _donationDetails.Clear();
        _donationDetails.AddRange(donationDetailsList);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Delete()
    {
        if (IsDeleted)
            return UnitResult.Success<Error>();

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Restore()
    {
        IsDeleted = false;
        DeletedAt = null;

        return UnitResult.Success<Error>();
    }

    private void EnsureMainPhoto()
    {
        if (_photos.Count > 0 && _photos.All(photo => !photo.IsMain))
            _photos[0] = _photos[0].WithIsMain(true);
    }
}
