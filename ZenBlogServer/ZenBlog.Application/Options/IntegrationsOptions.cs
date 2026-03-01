namespace ZenBlog.Application.Options;

public sealed class RedisOptions
{
    public bool Enabled { get; set; } = true;
    public string? ConnectionString { get; set; }
}

public sealed class RabbitMqOptions
{
    public bool Enabled { get; set; } = true;
    public string? ConnectionString { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "zenblog";
    public string Password { get; set; } = "zenblog";
}

public sealed class AwsS3Options
{
    public bool Enabled { get; set; } = false;
    public string Region { get; set; } = "eu-central-1";
    public string BucketName { get; set; } = "zenblog-bucket";
    public string KeyPrefix { get; set; } = "uploads/";
    public bool MakePublic { get; set; } = true;
    public string? ServiceUrl { get; set; }
    public bool ForcePathStyle { get; set; } = true;
    public string? PublicBaseUrl { get; set; }
}

public sealed class StorageOptions
{
    public string Provider { get; set; } = "Local";
}
