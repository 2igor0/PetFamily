using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record PetPhoto
{
    private PetPhoto(string path, bool isMain)
    {
        Path = path;
        IsMain = isMain;
    }
            
    public string Path { get; }
    
    public bool IsMain { get; }

    public static Result<PetPhoto, Error> Create(string path, bool isMain)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Error.Validation("path.is.invalid", "path is null or empty");
        }
        
        return new PetPhoto(path, isMain);
    }
}