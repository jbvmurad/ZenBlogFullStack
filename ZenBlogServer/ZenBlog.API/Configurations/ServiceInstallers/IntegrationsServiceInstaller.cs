using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Options;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.Application.Options;
using ZenBlog.Application.Services.Caching;
using ZenBlog.Application.Services.Storage;
using ZenBlog.Infrastructure.Services.Caching;
using ZenBlog.Infrastructure.Services.Storage;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public sealed class IntegrationsServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        services.AddOptions();

        services.Configure<RedisOptions>(configuration.GetSection("Redis"));
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.Configure<AwsS3Options>(configuration.GetSection("AwsS3"));
        services.Configure<StorageOptions>(configuration.GetSection("Storage"));

        ConfigureRedis(services, configuration);
        ConfigureAwsS3(services, configuration);
        ConfigureStorage(services);
    }

    private static void ConfigureRedis(IServiceCollection services, IConfiguration configuration)
    {
        var redis = configuration.GetSection("Redis").Get<RedisOptions>() ?? new RedisOptions();
        var injected = configuration.GetConnectionString("redis");
        if (!string.IsNullOrWhiteSpace(injected))
            redis.ConnectionString = injected;

        if (redis.Enabled && !string.IsNullOrWhiteSpace(redis.ConnectionString))
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redis.ConnectionString;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddSingleton<ICacheService, RedisCacheService>();
    }

    private static void ConfigureAwsS3(IServiceCollection services, IConfiguration configuration)
    {
        var s3 = configuration.GetSection("AwsS3").Get<AwsS3Options>() ?? new AwsS3Options();

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var cfg = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3.Region)
            };

            if (!string.IsNullOrWhiteSpace(s3.ServiceUrl))
            {
                cfg.ServiceURL = s3.ServiceUrl;
                cfg.ForcePathStyle = s3.ForcePathStyle;
                cfg.AuthenticationRegion = s3.Region;
            }

            return new AmazonS3Client(cfg);
        });

        services.AddHostedService<ZenBlog.Infrastructure.Services.Storage.S3BucketInitializerHostedService>();
    }

    private static void ConfigureStorage(IServiceCollection services)
    {
        services.AddScoped<LocalFileStorage>();
        services.AddScoped<S3FileStorage>();

        services.AddScoped<IFileStorage>(sp =>
        {
            var storage = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
            var s3 = sp.GetRequiredService<IOptions<AwsS3Options>>().Value;

            if (storage.Provider.Equals("S3", StringComparison.OrdinalIgnoreCase) && s3.Enabled)
                return sp.GetRequiredService<S3FileStorage>();

            return sp.GetRequiredService<LocalFileStorage>();
        });
    }

}
