using GenericRepository;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.API.Middleware;
using ZenBlog.API.OptionsSetup;
using ZenBlog.Application.Jwt;
using ZenBlog.Application.Services.ExternalAuth;
using ZenBlog.Application.Services.MailService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Infrastructure.Authentication;
using ZenBlog.Persistance.Context;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public class InfrastructureServiceInstallers : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<ExceptionMiddleware>();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetups>();

        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ZenBlogContext>());
        var emailParams = configuration
            .GetSection("EmailParameters")
            .Get<EmailParameters>();
        services.AddSingleton(emailParams);

        var clientAppUrls = configuration
            .GetSection("ClientAppUrls")
            .Get<ClientAppUrls>();
        services.AddSingleton(clientAppUrls);

        var googleAuthOptions = configuration
            .GetSection("GoogleAuth")
            .Get<GoogleAuthOptions>();
        services.AddSingleton(googleAuthOptions);

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();

    }
}
