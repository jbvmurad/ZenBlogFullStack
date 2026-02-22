var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "zenblog")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres");

var zenblogDb = postgres.AddDatabase("zenblog");

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "zenblog")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "zenblog")
    .WithManagementPlugin();

// Local AWS S3 (LocalStack)
// We expose it on a fixed port to keep PublicBaseUrl stable for the frontend.
var localstack = builder.AddContainer("localstack", "localstack/localstack:3")
    .WithEnvironment("SERVICES", "s3")
    .WithEnvironment("AWS_ACCESS_KEY_ID", "test")
    .WithEnvironment("AWS_SECRET_ACCESS_KEY", "test")
    .WithEnvironment("AWS_REGION", "us-east-1")
    .WithHttpEndpoint(port: 4566, targetPort: 4566, name: "http", isProxied: false)
    .WithExternalHttpEndpoints();

// API
builder.AddProject<Projects.ZenBlog_API>("zenblog-api")
    // Postgres connection string -> ConnectionStrings:ZenBlogConnection
    .WithReference(zenblogDb, connectionName: "ZenBlogConnection")
    // Redis/RabbitMQ injected as ConnectionStrings:redis / ConnectionStrings:rabbitmq
    .WithReference(redis)
    .WithReference(rabbitmq)
    // App config for integrations
    .WithEnvironment("Redis__Enabled", "true")
    .WithEnvironment("RabbitMq__Enabled", "true")
    .WithEnvironment("RabbitMq__Username", "zenblog")
    .WithEnvironment("RabbitMq__Password", "zenblog")
    .WithEnvironment("AwsS3__Enabled", "true")
    .WithEnvironment("AwsS3__Region", "us-east-1")
    .WithEnvironment("AwsS3__BucketName", "zenblog-local")
    .WithEnvironment("AwsS3__KeyPrefix", "uploads/")
    .WithEnvironment("AwsS3__MakePublic", "true")
    .WithEnvironment("AwsS3__ServiceUrl", "http://localhost:4566")
    .WithEnvironment("AwsS3__ForcePathStyle", "true")
    .WithEnvironment("AwsS3__PublicBaseUrl", "http://localhost:4566/zenblog-local")
    .WithEnvironment("Storage__Provider", "S3")
    // Email (Gmail SMTP): set these as environment variables when running the AppHost
    //   GMAIL_USER=you@gmail.com
    //   GMAIL_APP_PASSWORD=xxxx xxxx xxxx xxxx
    .WithEnvironment("EmailParameters__Host", "smtp.gmail.com")
    .WithEnvironment("EmailParameters__Port", "587")
    .WithEnvironment("EmailParameters__EnableSsl", "true")
    .WithEnvironment("EmailParameters__From", Environment.GetEnvironmentVariable("GMAIL_USER") ?? "")
    .WithEnvironment("EmailParameters__Username", Environment.GetEnvironmentVariable("GMAIL_USER") ?? "")
    .WithEnvironment("EmailParameters__AppPassword", Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD") ?? "")
    // AWS creds for LocalStack
    .WithEnvironment("AWS_ACCESS_KEY_ID", "test")
    .WithEnvironment("AWS_SECRET_ACCESS_KEY", "test")
    .WithEnvironment("AWS_REGION", "us-east-1")
    .WaitFor(zenblogDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WaitFor(localstack);

builder.Build().Run();
