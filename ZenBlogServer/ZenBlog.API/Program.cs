using Serilog;
using ZenBlog.API.Configurations;
using ZenBlog.API.Configurations.AppInstallers;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.API.SeriLog;

try
{
    Log.Logger = SerilogSetup.CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .InstallServices(
            builder.Configuration,
            builder.Host,
            typeof(IServiceInstaller).Assembly);

    builder.AddAppSerilog();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    await app.ApplyStartupDataAsync();
    app.MapZenBlogEndpoints();
    app.UseZenBlogRequestPipeline();
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
