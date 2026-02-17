using LetsSpeakLaw.API.Middleware;
using Scalar.AspNetCore;
using Serilog;
using ZenBlog.API.Configurations;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.API.SeriLog;

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

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options => options
            .AddPreferredSecuritySchemes("Bearer")
            .EnablePersistentAuthentication());
    }

    app.UseHttpsRedirection();

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
