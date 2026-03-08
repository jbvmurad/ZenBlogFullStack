namespace ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.DeleteUserRole;

public sealed record DeleteUserRoleCommand(
    List<string> RoleIds);
