using JasperFx.Resources;
using ZenBlog.Application.Options;
using ZenBlog.Domain.Events;
using Wolverine;
using Wolverine.RabbitMQ;

namespace ZenBlog.API.Configurations.ServiceInstallers;

public static class WolverineMessagingServiceInstaller
{
    public static void ConfigureRabbitMqMessaging(this WolverineOptions opts, IConfiguration configuration)
    {
        var rabbit = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        var injected = configuration.GetConnectionString("rabbitmq");
        if (!string.IsNullOrWhiteSpace(injected))
            rabbit.ConnectionString = injected;

        if (!rabbit.Enabled)
            return;

        var uri = BuildAmqpUri(rabbit);

        opts.UseRabbitMq(new Uri(uri)).AutoProvision();
        opts.Services.AddResourceSetupOnStartup();

        opts.ListenToRabbitQueue("zenblog.blog-created");

        opts.PublishMessage<BlogCreatedIntegrationEvent>().ToRabbitQueue("zenblog.blog-created");
    }

    private static string BuildAmqpUri(RabbitMqOptions rabbit)
    {
        if (!string.IsNullOrWhiteSpace(rabbit.ConnectionString))
        {
            var raw = rabbit.ConnectionString.Trim();

            if (!raw.Contains("://", StringComparison.Ordinal))
                raw = $"amqp://{raw}";

            if (raw.StartsWith("rabbitmq://", StringComparison.OrdinalIgnoreCase))
                raw = "amqp://" + raw.Substring("rabbitmq://".Length);

            return raw;
        }

        var vhost = string.IsNullOrWhiteSpace(rabbit.VirtualHost) || rabbit.VirtualHost == "/"
            ? "%2F"
            : Uri.EscapeDataString(rabbit.VirtualHost.TrimStart('/'));

        return $"amqp://{Uri.EscapeDataString(rabbit.Username)}:{Uri.EscapeDataString(rabbit.Password)}@{rabbit.Host}:{rabbit.Port}/{vhost}";
    }
}
