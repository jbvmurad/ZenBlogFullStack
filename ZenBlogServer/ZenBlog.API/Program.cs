using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;
using Serilog;
using ZenBlog.API.Configurations;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.API.Middleware;
using ZenBlog.API.SeriLog;
using ZenBlog.Persistance.Context;

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .InstallServices(
            builder.Configuration,
            builder.Host,
            typeof(IServiceInstaller).Assembly);

    builder.AddAppSerilog();

    var keysPath = builder.Configuration["DataProtection:KeysPath"];
    if (!string.IsNullOrWhiteSpace(keysPath))
    {
        Directory.CreateDirectory(keysPath);
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
    }

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Configuration.GetValue("Database:MigrateOnStartup", false))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZenBlogContext>();
        db.Database.Migrate();
    }

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
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
