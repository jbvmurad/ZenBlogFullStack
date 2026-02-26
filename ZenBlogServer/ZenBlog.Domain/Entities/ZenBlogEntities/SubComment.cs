using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.ZenBlogEntities;

public sealed class SubComment :BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Body { get; set; }

    public string CommentId { get; set; }
    public Comment Comment { get; set; }
}
