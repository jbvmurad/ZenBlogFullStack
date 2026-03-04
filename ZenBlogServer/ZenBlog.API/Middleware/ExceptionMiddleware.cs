using FluentValidation;
using ZenBlog.Domain.Entities.SystemEntities;
using ZenBlog.Persistance.Context;

namespace ZenBlog.API.Middleware;

public sealed class ExceptionMiddleware : IMiddleware
{
    private readonly ZenBlogContext _letsspeaklawContext;

    public ExceptionMiddleware(ZenBlogContext letsspeaklawContext)
    {
        _letsspeaklawContext = letsspeaklawContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
           await LogExceptionToDatabaseAsync(ex, context.Request);
           await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError &&
            ex.Message?.Contains("already taken", StringComparison.OrdinalIgnoreCase) == true)
        {
            statusCode = StatusCodes.Status409Conflict;
        }

        context.Response.StatusCode = statusCode;

        if (ex is ValidationException vex)
        {
            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return context.Response.WriteAsJsonAsync(new ValidationErrorDetails
            {
                Errors = errors,
                StatusCode = statusCode
            });
        }

        return context.Response.WriteAsJsonAsync(new ErrorResult
        {
            StatusCode = statusCode,
            Message = ex.Message
        });
    }
    private async Task LogExceptionToDatabaseAsync(Exception ex, HttpRequest request)
    {
        _letsspeaklawContext.ChangeTracker.Clear();

        ErrorLog errorLog = new()
        {
            ErrorMessage = ex.Message,
            StackTrace = ex.StackTrace,
            RequestPath = request.Path,
            RequestMethod = request.Method,
            Timestamp = DateTime.UtcNow,
        };

        await _letsspeaklawContext.Set<ErrorLog>().AddAsync(errorLog);
        await _letsspeaklawContext.SaveChangesAsync();
    }

}
