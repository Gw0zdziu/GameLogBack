
using GameLogBack.Constants;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;
        if (exception is AppException appException)
        {
            problem = new ProblemDetails
            {
                Status = (int)appException.StatusCode,
                Title = appException.Message,
                Extensions =
                {
                    ["code"] = appException.ErrorCode
                }
            };
            _logger.LogWarning(exception, message: appException.Message);
        }
        else
        {
            problem = new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
                Extensions =
                {
                    ["code"] = ErrorCodes.Internal.InternalServerError
                }
            };
            _logger.LogError(exception.ToString());
        }
        httpContext.Response.StatusCode = problem.Status!.Value;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;

    }
}