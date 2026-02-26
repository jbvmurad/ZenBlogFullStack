namespace ZenBlog.Domain.DTOs.UserDTOs;

public sealed record ClientAppUrls(
    string BaseUrl,
    string VerifyEmailPath,
    string ResetPasswordPath);
