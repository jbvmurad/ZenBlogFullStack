using Amazon;
using Amazon.S3;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using ZenBlog.API.Configurations.Abstraction;
using ZenBlog.API.Messaging.Consumers;
using ZenBlog.Application.Options;
using ZenBlog.Application.Services.Caching;
using ZenBlog.Application.Services.Storage;
using ZenBlog.Infrastructure.Services.Caching;
using ZenBlog.Infrastructure.Services.Storage;

namespace ZenBlog.API.Configurations.ServiceInstallers;

/// <summary>
/// Redis + RabbitMQ + AWS(S3) wiring.
/// </summary>
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
        ConfigureRabbitMq(services, configuration);
    }

    private static void ConfigureRedis(IServiceCollection services, IConfiguration configuration)
    {
        var redis = configuration.GetSection("Redis").Get<RedisOptions>() ?? new RedisOptions();

        // Prefer ConnectionStrings:redis if provided (Aspire injects it this way)
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
            // fallback so the app still works without Redis
            services.AddDistributedMemoryCache();
        }

        services.AddSingleton<ICacheService, RedisCacheService>();
    }

    private static void ConfigureAwsS3(IServiceCollection services, IConfiguration configuration)
    {
        var s3 = configuration.GetSection("AwsS3").Get<AwsS3Options>() ?? new AwsS3Options();

        // We register the client even if disabled, so switching providers is just config.
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var cfg = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3.Region)
            };

            if (!string.IsNullOrWhiteSpace(s3.ServiceUrl))
            {
                // LocalStack / custom endpoint
                cfg.ServiceURL = s3.ServiceUrl;
                cfg.ForcePathStyle = s3.ForcePathStyle;
                cfg.AuthenticationRegion = s3.Region;
            }

            // Uses the default AWS credentials chain (env vars, shared config, IAM role, etc.)
            return new AmazonS3Client(cfg);
        });

        // Ensures the bucket exists (helpful for LocalStack and fresh environments)
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

    private static void ConfigureRabbitMq(IServiceCollection services, IConfiguration configuration)
    {
        var rabbit = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        // Prefer ConnectionStrings:rabbitmq if provided (Aspire injects it this way)
        var injected = configuration.GetConnectionString("rabbitmq");
        if (!string.IsNullOrWhiteSpace(injected))
            rabbit.ConnectionString = injected;

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BlogCreatedConsumer>();

            if (rabbit.Enabled)
            {
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    if (!string.IsNullOrWhiteSpace(rabbit.ConnectionString))
                    {
                        // Accept amqp://, amqps:// or rabbitmq:// style URIs
                        var raw = rabbit.ConnectionString.Trim();
                        if (!raw.Contains("://", StringComparison.Ordinal))
                            raw = $"amqp://{raw}";

                        cfg.Host(new Uri(raw), h =>
                        {
                            // If creds are not part of the URI, fall back to config
                            if (!string.IsNullOrWhiteSpace(rabbit.Username))
                                h.Username(rabbit.Username);
                            if (!string.IsNullOrWhiteSpace(rabbit.Password))
                                h.Password(rabbit.Password);
                        });
                    }
                    else
                    {
                        // Build a URI to avoid relying on overload availability across versions.
                        // Note: default vhost '/' must be encoded as %2F in AMQP URIs.
                        var vhost = string.IsNullOrWhiteSpace(rabbit.VirtualHost) || rabbit.VirtualHost == "/"
                            ? "%2F"
                            : Uri.EscapeDataString(rabbit.VirtualHost.TrimStart('/'));

                        var uri = new Uri($"amqp://{rabbit.Host}:{rabbit.Port}/{vhost}");
                        cfg.Host(uri, h =>
                        {
                            h.Username(rabbit.Username);
                            h.Password(rabbit.Password);
                        });
                    }

                    cfg.ReceiveEndpoint("zenblog.blog-created", e =>
                    {
                        e.ConfigureConsumer<BlogCreatedConsumer>(ctx);
                    });
                });
            }
            else
            {
                x.UsingInMemory((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                });
            }
        });
    }
}
