using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Persistance.Context;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public class AuthorizeServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddAuthorization();

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ZenBlogContext>()
            .AddDefaultTokenProviders();
    }
}
