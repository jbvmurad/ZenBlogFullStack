using Microsoft.AspNetCore.DataProtection;
using ZenBlog.API.Configurations.Abstraction;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public sealed class DataProtectionServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        var keysPath = configuration["DataProtection:KeysPath"];
        if (string.IsNullOrWhiteSpace(keysPath))
            return;

        Directory.CreateDirectory(keysPath);

        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
    }
}
