using FluentValidation;
using Wolverine;
using Wolverine.FluentValidation;
using ZenBlog.API.Configurations.Abstraction;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public sealed class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services.AddWolverine(cfg =>
        {
            cfg.Discovery.IncludeAssembly(typeof(ApplicationServiceInstaller).Assembly);
            cfg.Discovery.IncludeAssembly(typeof(ZenBlog.Application.AssemblyReference).Assembly);

            cfg.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);

            cfg.ConfigureRabbitMqMessaging(configuration);
        });

        services.AddValidatorsFromAssembly(typeof(ZenBlog.Application.AssemblyReference).Assembly);
    }
}
