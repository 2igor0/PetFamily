

namespace PetFamily.Domain;

public class Pet
{
    //ef core
    private Pet()
    {
    }

    private Pet(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public Guid? Id { get; private set; }
    
    public string Name { get; private set; } = null!;
    
    public string Species { get; private set; } = null!; //вид, кошка или собака
    
    public string Description { get; private set; } = null!; // описание
    
    public string Breed { get; private set; } = null!; // порода
    
    public string Color { get; private set; } = null!;
    
    public string HealthInfo { get; private set; } = null!;

    public string CurrentAddress { get; private set; } = null!;

    public int Weight { get; private set; }

    public int Height { get; private set; }

    public int OwnerPhoneNumber { get; private set; } 

    public bool IsNeutered { get; private set; }  // кастрирован или нет

    public DateTime BirthDate { get; private set; } 

    public bool IsVaccinated { get; private set; } 
    
    public string HelpStatus { get; private set; } // Нуждается в помощи/Ищет дом/ Нашел дом
    
    public IReadOnlyList<DonationDetails> DonationDetails { get; private set; }
    
    public DateTime CreatedAt {get; private set;}

    public static Pet Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description));
        }
        
        var pet = new Pet(name, description);
        return pet;
        // сделать валидацию через библу экстеншон
    }
    
    // Дописать богатство
}