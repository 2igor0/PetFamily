using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts;
using PetFamily.API.Extensions;
using PetFamily.Application.FileStorage;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("minio-test")]
public class MinioTestController : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<ActionResult<string>> Upload(
        [FromForm] IFormFile file,
        [FromQuery] string? bucket,
        [FromServices] IFileStorage fileStorage,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return Error.Validation("file.is.empty", "File cannot be empty.").ToActionResult();

        var bucketName = string.IsNullOrWhiteSpace(bucket) ? FileStorageBuckets.Pets : bucket;
        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";

        await using var stream = file.OpenReadStream();

        var result = await fileStorage.UploadFile(
            stream,
            bucketName,
            fileName,
            file.ContentType,
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{bucket}/files")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetFiles(
        string bucket,
        [FromServices] IFileStorage fileStorage,
        CancellationToken cancellationToken)
    {
        var result = await fileStorage.GetFiles(bucket, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{bucket}/url/{*fileName}")]
    public async Task<ActionResult<string>> GetFileUrl(
        string bucket,
        string fileName,
        [FromServices] IFileStorage fileStorage,
        CancellationToken cancellationToken)
    {
        var result = await fileStorage.GetFileUrl(bucket, fileName, cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{bucket}/file/{*fileName}")]
    public async Task<ActionResult> DeleteFile(
        string bucket,
        string fileName,
        [FromServices] IFileStorage fileStorage,
        CancellationToken cancellationToken)
    {
        var result = await fileStorage.DeleteFile(bucket, fileName, cancellationToken);

        return result.ToActionResult();
    }
}
