using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.ZenBlogEntities;

public sealed class Comment:BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Body { get; set; }
    public  ICollection<SubComment> SubComments { get; set; }
    public string BlogId { get; set; }
    public  Blog Blog { get; set; }
}
