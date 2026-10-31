using System.Text.Json;
using PetFamily.API.Contracts;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Клиент отменил запрос — отвечать уже некому.
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var envelope = Envelope.Error(
                [Error.Failure("server.error", "Internal server error")]);

            await context.Response.WriteAsync(JsonSerializer.Serialize(envelope, JsonOptions));
        }
    }
}
