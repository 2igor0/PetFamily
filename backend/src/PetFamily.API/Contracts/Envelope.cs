using PetFamily.Domain.Shared;

namespace PetFamily.API.Contracts;

public record Envelope
{
    public object? Result { get; init; }

    public IReadOnlyList<ErrorInfo>? ErrorInfo { get; init; }

    public DateTime TimeGenerated { get; init; } = DateTime.UtcNow;

    public static Envelope Ok(object? result = null) => new()
    {
        Result = result
    };

    public static Envelope Error(IEnumerable<Error> errors) => new()
    {
        ErrorInfo = errors
            .Select(error => new ErrorInfo(error.Code, error.Message, error.InvalidField))
            .ToList()
    };
}

public record ErrorInfo(string? ErrorCode, string? ErrorMessage, string? InvalidField);
