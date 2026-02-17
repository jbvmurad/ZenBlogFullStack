using Microsoft.AspNetCore.OData;
using ZenBlog.API.Configurations.Abstraction;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public class PresentationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ZenBlog.Presentation.AssemblyReference).Assembly);

        services.AddEndpointsApiExplorer();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        services.AddControllers().AddOData(opt => opt
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(int.MaxValue)
        );
    }
}
