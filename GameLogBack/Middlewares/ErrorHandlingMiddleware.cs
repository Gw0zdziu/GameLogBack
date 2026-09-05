using GameLogBack.Exceptions;

namespace GameLogBack.Middlewares;

public class ErrorHandlingMiddleware: IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException e)
        {
            _logger.LogWarning(
                "Resource not found: {Message} | {Method} | {Path}",
                e.Message, context.Request.Method, context.Request.Path
                );
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(e.Message);
        }
        catch (BadRequestException e)
        {
            _logger.LogWarning(
                "Invalid request: {Message} | {Method} | {Path}",
                e.Message, context.Request.Method, context.Request.Path
            );
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(e.Message);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred |{Method} {Path} | TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
        }
    }
}