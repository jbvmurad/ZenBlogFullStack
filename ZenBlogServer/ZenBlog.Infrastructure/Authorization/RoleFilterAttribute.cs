using Microsoft.AspNetCore.Mvc;

namespace ZenBlog.Infrastructure.Authorization;

public sealed class RoleFilterAttribute : TypeFilterAttribute
{
    public RoleFilterAttribute(params string[] roles) : base(typeof(RoleAttribute))
    {
        Arguments = new object[] { roles };
    }
}
