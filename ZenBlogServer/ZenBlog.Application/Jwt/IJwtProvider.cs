using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Jwt;

public interface IJwtProvider
{
    Task<LoginCommandResponse> CreateTokenAsync(User user);
}
