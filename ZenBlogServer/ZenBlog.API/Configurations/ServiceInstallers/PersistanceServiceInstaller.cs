using Microsoft.EntityFrameworkCore;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.Persistance.Context;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public class PersistanceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(ZenBlog.Persistance.AssemblyReference).Assembly);
        });

        var connectionString = configuration.GetConnectionString("ZenBlogConnection");
        services.AddDbContext<ZenBlogContext>(options =>
            options.UseNpgsql(connectionString));
    }
}
