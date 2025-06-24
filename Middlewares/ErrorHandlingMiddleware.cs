using GameLogBack.Exceptions;

namespace GameLogBack.Middlewares;

public class ErrorHandlingMiddleware: IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException e)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(e.Message);
        }
        catch (BadRequestException e)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(e.Message);

        }
        catch (Exception e)
        {
            await context.Response.WriteAsync(e.Message);
        }
    }
}