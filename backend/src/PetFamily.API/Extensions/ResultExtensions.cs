using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T, Error> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(Envelope.Ok(result.Value))
            : FromError(result.Error);
    }

    public static ActionResult ToActionResult(this UnitResult<Error> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(Envelope.Ok())
            : FromError(result.Error);
    }

    public static ActionResult ToActionResult(this Error error) => FromError(error);

    private static ObjectResult FromError(Error error)
    {
        return new ObjectResult(Envelope.Error([error]))
        {
            StatusCode = GetStatusCode(error.Type)
        };
    }

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };
}
