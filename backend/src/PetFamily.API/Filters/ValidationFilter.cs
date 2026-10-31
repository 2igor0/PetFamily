using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PetFamily.API.Contracts;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Filters;

public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var modelStateErrors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error => Error.Validation(
                    "validation.error",
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Invalid value."
                        : error.ErrorMessage,
                    entry.Key)))
                .ToList();

            context.Result = new ObjectResult(Envelope.Error(modelStateErrors))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            return;
        }

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator is null)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            if (validationResult.IsValid)
                continue;

            var errors = validationResult.Errors
                .Select(failure => Error.Validation(
                    string.IsNullOrWhiteSpace(failure.ErrorCode) ? "validation.error" : failure.ErrorCode,
                    failure.ErrorMessage,
                    failure.PropertyName))
                .ToList();

            context.Result = new ObjectResult(Envelope.Error(errors))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            return;
        }

        await next();
    }
}
