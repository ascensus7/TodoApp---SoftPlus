using TodoApp.Services.Exceptions;

namespace TodoApp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status404NotFound,
                exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status409Conflict,
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status401Unauthorized,
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred.");

            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            error = message
        });
    }
}