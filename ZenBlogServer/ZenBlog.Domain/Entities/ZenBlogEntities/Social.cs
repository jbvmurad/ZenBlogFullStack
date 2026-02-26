using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.ZenBlogEntities;

public sealed class Social :BaseEntity
{
    public string Title { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
}
