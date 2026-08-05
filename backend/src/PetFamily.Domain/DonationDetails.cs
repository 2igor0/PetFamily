namespace PetFamily.Domain;

public class DonationDetails
{
    public DonationDetails()
    {
    }
    
    public DonationDetails(string name, string description)
    {
        if  (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrEmpty(description))
        {
            throw new ArgumentNullException(nameof(description));
        }
        
        this.Name = name;
        this.Description = description;
    }
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }
}