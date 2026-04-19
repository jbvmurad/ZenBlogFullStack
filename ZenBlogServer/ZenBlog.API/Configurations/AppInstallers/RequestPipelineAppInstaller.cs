using Microsoft.Extensions.FileProviders;
using ZenBlog.API.Middleware;

namespace ZenBlog.API.Configurations.AppInstallers;

public static class RequestPipelineAppInstaller
{
    public static void UseZenBlogRequestPipeline(this WebApplication app)
    {
        if (app.Configuration.GetValue("HttpsRedirection:Enabled", true))
            app.UseHttpsRedirection();

        var storageProvider = app.Configuration["Storage:Provider"] ?? "Local";
        if (storageProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
        {
            var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddlewareExtensions();
        app.MapControllers();
    }
}
