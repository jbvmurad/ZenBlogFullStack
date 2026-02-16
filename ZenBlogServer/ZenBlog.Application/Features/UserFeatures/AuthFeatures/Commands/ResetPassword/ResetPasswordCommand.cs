namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string UserId,
    string Token,
    string NewPassword) ;
