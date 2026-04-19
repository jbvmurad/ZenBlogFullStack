using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Application.Jwt;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Repositories.UserRepositories;

namespace ZenBlog.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<User> _userManager;
    private readonly IUserRoleRepository _userRoleRepository;

    public JwtProvider(
        IOptions<JwtOptions> jwtOptions,
        UserManager<User> userManager,
        IUserRoleRepository userRoleRepository)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<LoginCommandResponse> CreateTokenAsync(User user)
    {
        var roles = await _userRoleRepository
            .GetAll()
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
            .Select(x => x.Role.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToListAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
            new Claim("FullName", user.FullName ?? string.Empty),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role!)));

        DateTime expires = DateTime.UtcNow.AddHours(1);

        JwtSecurityToken jwtSecurityToken = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256));

        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = expires.AddMinutes(15);
        await _userManager.UpdateAsync(user);

        LoginCommandResponse response = new(
            token,
            refreshToken,
            user.RefreshTokenExpires,
            user.Id);

        return response;
    }
}
