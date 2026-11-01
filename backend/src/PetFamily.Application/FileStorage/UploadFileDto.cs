namespace PetFamily.Application.FileStorage;

public record UploadFileDto(
    Stream Content,
    string FileName,
    string ContentType,
    long Length);
