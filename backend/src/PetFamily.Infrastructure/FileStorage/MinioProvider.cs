using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FileStorage;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.FileStorage;

public sealed class MinioProvider(
    IMinioClient minioClient,
    IOptions<MinioOptions> options,
    ILogger<MinioProvider> logger) : IFileStorage
{
    private const int DefaultUrlExpirySeconds = 60 * 60;

    private readonly MinioOptions _options = options.Value;

    public Task<Result<string, Error>> UploadFile(
        Stream stream,
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return UploadFile(stream, bucketName, fileName, "application/octet-stream", cancellationToken);
    }

    public async Task<Result<string, Error>> UploadFile(
        Stream stream,
        string bucketName,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (stream is null || !stream.CanRead)
            return Error.Validation("file.stream.is.invalid", "File stream is invalid.");

        if (string.IsNullOrWhiteSpace(bucketName))
            return Error.Validation("file.bucket.is.invalid", "Bucket name cannot be empty.");

        if (string.IsNullOrWhiteSpace(fileName))
            return Error.Validation("file.name.is.invalid", "File name cannot be empty.");

        try
        {
            await EnsureBucketExists(bucketName, cancellationToken);

            var objectSize = stream.CanSeek ? stream.Length : -1;

            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(objectSize)
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(args, cancellationToken);

            return fileName;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to upload file {FileName} to bucket {BucketName}",
                fileName,
                bucketName);

            return Error.Failure("file.upload.failed", "Failed to upload file.");
        }
    }

    public async Task<UnitResult<Error>> DeleteFile(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return Error.Validation("file.bucket.is.invalid", "Bucket name cannot be empty.");

        if (string.IsNullOrWhiteSpace(fileName))
            return Error.Validation("file.name.is.invalid", "File name cannot be empty.");

        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            await minioClient.RemoveObjectAsync(args, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to delete file {FileName} from bucket {BucketName}",
                fileName,
                bucketName);

            return Error.Failure("file.delete.failed", "Failed to delete file.");
        }
    }

    public async Task<Result<string, Error>> GetFileUrl(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return Error.Validation("file.bucket.is.invalid", "Bucket name cannot be empty.");

        if (string.IsNullOrWhiteSpace(fileName))
            return Error.Validation("file.name.is.invalid", "File name cannot be empty.");

        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithExpiry(DefaultUrlExpirySeconds);

            var url = await minioClient.PresignedGetObjectAsync(args);

            return url;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to create presigned URL for file {FileName} in bucket {BucketName}",
                fileName,
                bucketName);

            return Error.Failure("file.url.failed", "Failed to create file URL.");
        }
    }

    public async Task<Result<IReadOnlyList<string>, Error>> GetFiles(
        string bucketName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return Error.Validation("file.bucket.is.invalid", "Bucket name cannot be empty.");

        try
        {
            var args = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithRecursive(true);

            var files = new List<string>();

            await foreach (var item in minioClient.ListObjectsEnumAsync(args, cancellationToken))
                files.Add(item.Key);

            return files;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to list files in bucket {BucketName}", bucketName);

            return Error.Failure("file.list.failed", "Failed to list files.");
        }
    }

    private async Task EnsureBucketExists(string bucketName, CancellationToken cancellationToken)
    {
        var bucketExists = await minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucketName),
            cancellationToken);

        if (!bucketExists)
        {
            await minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucketName),
                cancellationToken);
        }
    }
}
