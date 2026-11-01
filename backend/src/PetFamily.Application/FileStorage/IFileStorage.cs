using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.FileStorage;

public interface IFileStorage
{
    Task<Result<string, Error>> UploadFile(
        Stream stream,
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Result<string, Error>> UploadFile(
        Stream stream,
        string bucketName,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> DeleteFile(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Result<string, Error>> GetFileUrl(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<string>, Error>> GetFiles(
        string bucketName,
        CancellationToken cancellationToken = default);
}

public static class FileStorageBuckets
{
    public const string Pets = "pets";
}
