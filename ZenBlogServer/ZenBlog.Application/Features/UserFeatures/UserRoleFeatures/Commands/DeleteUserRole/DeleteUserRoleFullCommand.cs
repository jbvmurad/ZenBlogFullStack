namespace ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.DeleteUserRole;

public sealed record DeleteUserRoleFullCommand(
    string UserId,
    List<string> RoleIds
) ;
