using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Repositories.UserRepositories;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Repositories.UserRepositories;
using ZenBlog.Persistance.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Services.UserServices;
using ZenBlog.Persistance.Services.ZenBlogServices;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public class PersistanceDIServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IContactInfoService, ContactInfoService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<ISocialService, SocialService>();
        services.AddScoped<ISubCommentService, SubCommentService>();

        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ISocialRepository, SocialRepository>();
        services.AddScoped<ISubCommentRepository, SubCommentRepository>();
    }   
}
