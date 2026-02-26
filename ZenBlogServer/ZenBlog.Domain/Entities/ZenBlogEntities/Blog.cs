using ZenBlog.Domain.Entities.Abstraction;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Domain.Entities.ZenBlogEntities;

public sealed class Blog :BaseEntity
{
    public string Title { get; set; }
    public string CoverImage { get; set; }
    public string BlogImage { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public Category Category { get; set; }
    public string UserId { get; set; }
    public  User User { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
