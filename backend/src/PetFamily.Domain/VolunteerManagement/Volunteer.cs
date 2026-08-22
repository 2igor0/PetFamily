using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement.ValueObjects;

namespace PetFamily.Domain.VolunteerManagement;

public class Volunteer : Shared.Entity<IdVolunteer> //Для обновления модели должны быть отдельные методы.
{
    // ef core
    private Volunteer() : base(IdVolunteer.Empty())
    {
        //...
    }

    private Volunteer(FullName fullName,
        Description description,
        PhysicalAttributes yearsOfExperience,
        PhoneNumber phoneNumber,
        IdVolunteer idVolunteer,
        IEnumerable<SocialNetwork>? socialNetwork,
        IEnumerable<DonationDetails>? donationDetails) : base(idVolunteer)
    {
        FullName = fullName;
        Description = description;
        YearsOfExperience = yearsOfExperience;
        OwnerPhoneNumber = phoneNumber;
        if (socialNetwork != null) _socialNetworks = socialNetwork.ToList(); // хз
        if (donationDetails != null) _donationDetails = donationDetails.ToList(); // хз
    }

    public static Result<Volunteer, Error> Create(
        FullName fullName, //тут надо как-то ошибку выводить, но хз зачем и почему, если у нас все прошло валидацию
        Description description,
        PhysicalAttributes yearsOfExperience,
        PhoneNumber phoneNumber,
        IdVolunteer idVolunteer,
        IEnumerable<SocialNetwork>? socialNetwork,
        IEnumerable<DonationDetails>? donationDetails)
    {
        return new Volunteer(fullName, description, yearsOfExperience, phoneNumber, idVolunteer, socialNetwork,
            donationDetails);
    }

    public FullName FullName { get; private set; }

    public Description Description { get; private set; }

    public PhysicalAttributes YearsOfExperience { get; private set; }

    public PhoneNumber OwnerPhoneNumber { get; private set; }


    private readonly List<DonationDetails> _donationDetails = [];

    public IReadOnlyList<DonationDetails> DonationDetails => _donationDetails;


    private readonly List<SocialNetwork> _socialNetworks = [];

    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;


    private readonly List<Pet> _pets = [];

    public IReadOnlyList<Pet> Pets => _pets;


    public Result AddDonationDetails(DonationDetails? details) // дописать потом правильн с ошибками Error
    {
        if (details is null)
        {
            return Result.Failure("details is invalid");
        }

        _donationDetails.Add(details);

        return Result.Success();
    }

    public Result AddSocialNetwork(SocialNetwork? socialNetwork)
    {
        if (socialNetwork is null)
        {
            return Result.Failure("socialNetwork is invalid");
        }

        _socialNetworks.Add(socialNetwork);

        return Result.Success();
    }

    public Result AddPet(Pet? pet) // Result
    {
        if (pet is null)
        {
            return Result.Failure("pet is invalid");
        }

        _pets.Add(pet);

        return Result.Success();
    }

    public int GetAdoptedPetsCount()
    {
        // ...
        return 0;
    }

    public int GetPetsSeekingHomeCount()
    {
        // ...
        return 0;
    }

    public int GetPetsUnderTreatmentCount()
    {
        // ...
        return 0;
    }
}