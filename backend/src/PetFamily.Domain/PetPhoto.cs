namespace PetFamily.Domain;

public record PetPhoto
{
    public PetPhoto(string path, bool isMain)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        
        Path = path;
        IsMain = isMain;
    }
            
    public string Path { get; set; } = null!;
    
    public bool IsMain { get; set; }
}