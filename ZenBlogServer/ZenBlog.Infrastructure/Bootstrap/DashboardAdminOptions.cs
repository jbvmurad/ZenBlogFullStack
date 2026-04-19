namespace ZenBlog.Infrastructure.Bootstrap;

public sealed class DashboardAdminOptions
{
    public bool Enabled { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? FullName { get; init; }
    public string? UserName { get; init; }
}
