using Scalar.AspNetCore;

namespace ZenBlog.API.Configurations.AppInstallers;

public static class EndpointAppInstaller
{
    public static void MapZenBlogEndpoints(this WebApplication app)
    {
        var openApiEnabled = app.Configuration.GetValue("OpenApi:Enabled", false) || app.Environment.IsDevelopment();
        if (openApiEnabled)
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options => options
                .AddPreferredSecuritySchemes("Bearer")
                .EnablePersistentAuthentication());
        }

        app.MapGet("/", () => Results.Ok(new { service = "ZenBlog.API", status = "ok" }));
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
    }
}
