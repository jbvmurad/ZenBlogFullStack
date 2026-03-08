namespace ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.GiveUserRole;

public sealed record GiveUserRoleCommand(
    string UserId,
    string RoleId) ;
