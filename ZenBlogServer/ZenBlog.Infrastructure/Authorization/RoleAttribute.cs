using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ZenBlog.Infrastructure.Authorization;

public sealed class RoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public RoleAttribute(string[] roles)
    {
        _roles = roles
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_roles.Length == 0)
        {
            return;
        }

        var userHasRole = _roles.Any(context.HttpContext.User.IsInRole);
        
        if (!userHasRole)
        {
            context.Result = new ForbidResult();
        }
    }
}
