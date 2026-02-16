using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.SystemEntities;

public sealed class ErrorLog :BaseEntity
{
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
    public string RequestPath { get; set; }
    public string RequestMethod { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
