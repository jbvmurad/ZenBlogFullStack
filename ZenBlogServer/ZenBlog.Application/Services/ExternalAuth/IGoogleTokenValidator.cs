using ZenBlog.Domain.DTOs.UserDTOs;

namespace ZenBlog.Application.Services.ExternalAuth;

public interface IGoogleTokenValidator
{
    Task <GoogleUserInfo> ValidateAsync(string idToken,CancellationToken cancellationToken);
}
