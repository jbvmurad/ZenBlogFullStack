var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ZenBlog_API>("zenblog-api");

builder.Build().Run();
