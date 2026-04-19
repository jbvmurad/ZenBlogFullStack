using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Infrastructure.Bootstrap;

public sealed class DashboardAdminSeeder
{
    private const string AdminRoleName = "Admin";

    private readonly IOptions<DashboardAdminOptions> _options;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ZenBlogContext _context;
    private readonly ILogger<DashboardAdminSeeder> _logger;

    public DashboardAdminSeeder(
        IOptions<DashboardAdminOptions> options,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ZenBlogContext context,
        ILogger<DashboardAdminSeeder> logger)
    {
        _options = options;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var options = _options.Value;

        if (!options.Enabled)
        {
            return;
        }

        var email = options.Email?.Trim();
        var password = options.Password?.Trim();
        var userName = string.IsNullOrWhiteSpace(options.UserName)
            ? email
            : options.UserName.Trim();
        var fullName = string.IsNullOrWhiteSpace(options.FullName)
            ? "Dashboard Admin"
            : options.FullName.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Dashboard admin seeding is enabled, but email/password values are missing.");
            return;
        }

        var adminRole = await EnsureAdminRoleAsync();
        var user = await FindDashboardAdminAsync(email, userName);

        if (user is null)
        {
            user = new User
            {
                Email = email,
                UserName = userName,
                FullName = fullName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "dashboard admin user");
        }
        else
        {
            var requiresUpdate = false;

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = email;
                user.NormalizedEmail = _userManager.NormalizeEmail(email);
                requiresUpdate = true;
            }

            if (!string.Equals(user.UserName, userName, StringComparison.Ordinal))
            {
                user.UserName = userName;
                user.NormalizedUserName = _userManager.NormalizeName(userName);
                requiresUpdate = true;
            }

            if (!string.Equals(user.FullName, fullName, StringComparison.Ordinal))
            {
                user.FullName = fullName;
                requiresUpdate = true;
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                requiresUpdate = true;
            }

            if (requiresUpdate)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                EnsureSucceeded(updateResult, "dashboard admin user");
            }

            var passwordMatches = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordMatches)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
                EnsureSucceeded(resetResult, "dashboard admin password");
            }
        }

        var hasAdminRoleLink = await _context.Set<UserRole>()
            .AnyAsync(x => x.UserId == user.Id && x.RoleId == adminRole.Id, cancellationToken);

        if (!hasAdminRoleLink)
        {
            await _context.Set<UserRole>().AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Dashboard admin account is ready for {Email}.", email);
    }

    private async Task<Role> EnsureAdminRoleAsync()
    {
        var role = await _roleManager.FindByNameAsync(AdminRoleName);
        if (role is not null)
        {
            return role;
        }

        role = new Role
        {
            Name = AdminRoleName,
            NormalizedName = _roleManager.NormalizeKey(AdminRoleName)
        };

        var createResult = await _roleManager.CreateAsync(role);
        EnsureSucceeded(createResult, "admin role");

        return role;
    }

    private async Task<User?> FindDashboardAdminAsync(string email, string? userName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return user;
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }

        return await _userManager.FindByNameAsync(userName);
    }

    private static void EnsureSucceeded(IdentityResult result, string subject)
    {
        if (result.Succeeded)
        {
            return;
        }

        var message = string.Join(" | ", result.Errors.Select(x => x.Description));
        throw new InvalidOperationException($"Failed to prepare {subject}: {message}");
    }
}
