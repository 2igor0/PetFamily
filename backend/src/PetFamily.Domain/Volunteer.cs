namespace PetFamily.Domain;

public class Volunteer // доделать богатство 
{
    public Guid Id { get; private set; }
    
    public record FullName(string FirstName, string LastName, string? Patronymic);
    
    public string Description { get; private set; } = null!;
    
    public int YearsOfExperience { get; private set; }
    
    public int OwnerPhoneNumber { get; private set; } 
    

    private readonly List<DonationDetails> _donationDetails = [];

    public IReadOnlyList<DonationDetails> DonationDetails => _donationDetails;
    
    
    private readonly List<SocialNetwork> _socialNetworks = []; 
    
    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;
    
    
    private readonly List<Pet> _pets = []; 

    public IReadOnlyList<Pet> Pets => _pets;
    

    public void AddDonationDetails(DonationDetails? details)
    {
        ArgumentNullException.ThrowIfNull(details);
        _donationDetails.Add(details);
    }

    public void AddSocialNetwork(SocialNetwork? socialNetwork)
    {
        ArgumentNullException.ThrowIfNull(socialNetwork);
        _socialNetworks.Add(socialNetwork);
    }

    public void AddPet(Pet? pet)
    {
        ArgumentNullException.ThrowIfNull(pet);
        _pets.Add(pet);
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