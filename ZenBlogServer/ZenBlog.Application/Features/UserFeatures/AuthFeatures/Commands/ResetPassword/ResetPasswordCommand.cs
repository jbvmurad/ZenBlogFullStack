namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string UserId,
    string Token,
    string NewPassword,
    string ConfirmPassword);
