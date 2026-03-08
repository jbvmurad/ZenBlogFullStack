namespace ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.DeleteUserRole;

public sealed record DeleteUserRoleFullCommand(
    string UserId,
    List<string> RoleIds
) ;
