namespace ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.DeleteUserRole;

public sealed record DeleteUserRoleCommand(
    List<string> RoleIds);
