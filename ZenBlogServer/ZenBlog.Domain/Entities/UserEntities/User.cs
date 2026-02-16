using Microsoft.AspNetCore.Identity;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Domain.Entities.UserEntities;

public sealed class User : IdentityUser<string>
{
    public User()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string FullName { get; set; }
    public string RefreshToken { get; set; }

#nullable enable
    public string? ImageUrl { get; set; }
#nullable disable

    public DateTime? RefreshTokenExpires { get; set; }
    public ICollection<Blog> Blogs { get; set; }

}
