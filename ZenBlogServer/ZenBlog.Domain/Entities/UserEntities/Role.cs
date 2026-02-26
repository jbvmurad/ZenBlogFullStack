using Microsoft.AspNetCore.Identity;

namespace ZenBlog.Domain.Entities.UserEntities;

public sealed class Role :IdentityRole<string>
{
    public Role()
    {
        Id=Guid.NewGuid().ToString();
    }
}
