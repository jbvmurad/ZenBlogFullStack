using FluentValidation;
using GenericRepository;
using LetsSpeakLaw.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.FluentValidation;
using ZenBlog.API.Middleware;
using ZenBlog.API.OptionsSetup;
using ZenBlog.API.SeriLog;
using ZenBlog.Application.Jwt;
using ZenBlog.Application.Services.ExternalAuth;
using ZenBlog.Application.Services.MailService;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Repositories.UserRepositories;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Infrastructure.Authentication;
using ZenBlog.Persistance.Context;
using ZenBlog.Persistance.Repositories.UserRepositories;
using ZenBlog.Persistance.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Services.UserServices;
using ZenBlog.Persistance.Services.ZenBlogServices;

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.AddAppSerilog();

    builder.Services
        .AddControllers()
        .AddApplicationPart(typeof(ZenBlog.Presentation.AssemblyReference).Assembly);

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

    builder.Services.AddAuthorization();

    builder.Services.AddWolverine(cfg =>
    {
        cfg.Discovery.IncludeAssembly(typeof(ZenBlog.Application.AssemblyReference).Assembly);

        cfg.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
    });

    builder.Services.AddValidatorsFromAssembly(typeof(ZenBlog.Application.AssemblyReference).Assembly);


    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IUserRoleService, UserRoleService>();
    builder.Services.AddScoped<IBlogService, BlogService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IContactInfoService, ContactInfoService>();
    builder.Services.AddScoped<IMessageService, MessageService>();
    builder.Services.AddScoped<ISocialService, SocialService>();
    builder.Services.AddScoped<ISubCommentService, SubCommentService>();


    builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IBlogRepository, BlogRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
    builder.Services.AddScoped<IMessageRepository, MessageRepository>();
    builder.Services.AddScoped<ISocialRepository, SocialRepository>();
    builder.Services.AddScoped<ISubCommentRepository, SubCommentRepository>();

    builder.Services.AddIdentity<User, Role>()
        .AddEntityFrameworkStores<ZenBlogContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddScoped<IJwtProvider, JwtProvider>();
    builder.Services.AddScoped<ExceptionMiddleware>();
    builder.Services.ConfigureOptions<JwtOptionsSetup>();
    builder.Services.ConfigureOptions<JwtBearerOptionsSetups>();

    builder.Services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ZenBlogContext>());
    var emailParams = builder.Configuration
        .GetSection("EmailParameters")
        .Get<EmailParameters>();
    builder.Services.AddSingleton(emailParams);

    var clientAppUrls = builder.Configuration
        .GetSection("ClientAppUrls")
        .Get<ClientAppUrls>();
    builder.Services.AddSingleton(clientAppUrls);

    var googleAuthOptions = builder.Configuration
        .GetSection("GoogleAuth")
        .Get<GoogleAuthOptions>();
    builder.Services.AddSingleton(googleAuthOptions);

    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.AddMaps(typeof(ZenBlog.Persistance.AssemblyReference).Assembly);
    });

    var connectionString = builder.Configuration.GetConnectionString("ZenBlogConnection");
    builder.Services.AddDbContext<ZenBlogContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddControllers().AddOData(opt => opt
    .Select()
    .Filter()
    .OrderBy()
    .Expand()
    .Count()
    .SetMaxTop(int.MaxValue)
    );

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
