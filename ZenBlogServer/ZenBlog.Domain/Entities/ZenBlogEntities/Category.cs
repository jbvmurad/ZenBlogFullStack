using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.ZenBlogEntities;

public sealed class Category :BaseEntity
{
    public string Name { get; set; }
    public ICollection<Blog> Blogs { get; set; }
}
