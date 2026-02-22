namespace ZenBlog.Application.Options;

public sealed class RedisOptions
{
    public bool Enabled { get; set; } = true;
    public string? ConnectionString { get; set; }
}

public sealed class RabbitMqOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Optional full connection string (e.g. amqp://user:pass@host:5672/vhost).
    /// If provided (or injected by Aspire as ConnectionStrings:rabbitmq), it takes precedence.
    /// </summary>
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

    /// <summary>
    /// If true, objects will be uploaded with PublicRead ACL (works only if your bucket/account policy allows ACLs).
    /// If false, you'll typically want to use CloudFront signed URLs or S3 pre-signed URLs.
    /// </summary>
    public bool MakePublic { get; set; } = true;

    // For local development (LocalStack). Leave null/empty for real AWS.
    public string? ServiceUrl { get; set; }
    public bool ForcePathStyle { get; set; } = true;

    // If your bucket is public or fronted by CloudFront, this is the public base URL.
    // Example: https://<bucket>.s3.<region>.amazonaws.com
    public string? PublicBaseUrl { get; set; }
}

public sealed class StorageOptions
{
    /// <summary>
    /// "Local" or "S3"
    /// </summary>
    public string Provider { get; set; } = "Local";
}
