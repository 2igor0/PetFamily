using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagement.ValueObjects;

public record PetPhoto
{
    public string Path { get; }

    public bool IsMain { get; }

    private PetPhoto()
    {
        Path = null!;
    }

    private PetPhoto(string path, bool isMain)
    {
        Path = path;
        IsMain = isMain;
    }

    public static Result<PetPhoto, Error> Create(string? path, bool isMain)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Error.Validation("pet.photo.path.is.invalid", "Photo path cannot be empty.");

        var trimmedPath = path.Trim();

        if (trimmedPath.Length > Constants.MaxMediumTextLength)
            return Error.Validation(
                "pet.photo.path.is.too.long",
                $"Photo path cannot exceed {Constants.MaxMediumTextLength} characters.");

        return new PetPhoto(trimmedPath, isMain);
    }

    public PetPhoto WithIsMain(bool isMain) => new(Path, isMain);
}
