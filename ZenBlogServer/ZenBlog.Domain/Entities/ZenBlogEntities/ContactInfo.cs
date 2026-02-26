using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.ZenBlogEntities;

public sealed class ContactInfo :BaseEntity
{
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string MapUrl { get; set; }
}
