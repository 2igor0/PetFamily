namespace PetFamily.Domain.Shared;

public record Error
{
    public string Code { get; }
    
    public string Message { get; }
    
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }
    
    public static Error Validation(string code, string message) =>
        new Error(code, message, ErrorType.Validation);
    
    public static Error NotFound(string code, string message) =>
        new Error(code, message, ErrorType.NotFound);
    
    public static Error Conflict(string code, string message) =>
        new Error(code, message, ErrorType.Conflict);
    
    public static Error Failu(string code, string message) =>
        new Error(code, message, ErrorType.Failure);
    
}

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Failure
}