using ZenBlog.API.Middleware;

namespace LetsSpeakLaw.API.Middleware;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseMiddlewareExtensions(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
