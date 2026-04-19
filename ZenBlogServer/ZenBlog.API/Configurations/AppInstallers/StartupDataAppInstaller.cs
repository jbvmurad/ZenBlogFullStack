using Microsoft.EntityFrameworkCore;
using ZenBlog.Infrastructure.Bootstrap;
using ZenBlog.Persistance.Context;

namespace ZenBlog.API.Configurations.AppInstallers;

public static class StartupDataAppInstaller
{
    public static async Task ApplyStartupDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ZenBlogContext>();

        if (app.Configuration.GetValue("Database:MigrateOnStartup", false))
        {
            await db.Database.MigrateAsync();
        }

        var dashboardAdminSeeder = scope.ServiceProvider.GetRequiredService<DashboardAdminSeeder>();
        await dashboardAdminSeeder.SeedAsync();
    }
}
