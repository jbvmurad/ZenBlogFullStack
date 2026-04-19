using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wolverine;
using ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.DeleteUserRole;
using ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.GiveUserRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Infrastructure.Authorization;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.UserControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UserRoleController : APIController
{
    private readonly IUserRoleService _userRoleService;
    private readonly IAuthService _authService;
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public UserRoleController(
        IMessageBus bus,
        IUserRoleService userRoleService,
        IAuthService authService,
        IRoleService roleService,
        IMapper mapper,
        IConfiguration configuration) : base(bus)
    {
        _userRoleService = userRoleService;
        _authService = authService;
        _roleService = roleService;
        _mapper = mapper;
        _configuration = configuration;
    }

    [RoleFilter("Admin", "Manager")]
    [HttpGet]
    public async Task<IActionResult> GetAll(ODataQueryOptions<UserRole> options, CancellationToken cancellationToken)
    {
        IQueryable<UserRole> query = _userRoleService.GetAllUserRoles();

        query = (IQueryable<UserRole>)options.ApplyTo(query, new ODataQuerySettings());

        var result = await query
            .ProjectTo<UserRoleResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [RoleFilter("Admin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteRoles([FromRoute] string userId, [FromBody] DeleteUserRoleBody body)
    {
        var protectedEmail = _configuration["DashboardAdmin:Email"]?.Trim();
        var protectedUserName = _configuration["DashboardAdmin:UserName"]?.Trim();
        var normalizedProtectedEmail = string.IsNullOrWhiteSpace(protectedEmail) ? null : protectedEmail.ToLower();
        var normalizedProtectedUserName = string.IsNullOrWhiteSpace(protectedUserName) ? null : protectedUserName.ToLower();

        var isProtectedAdminRoleRemoval = await _userRoleService
            .GetAllUserRoles()
            .Include(x => x.User)
            .Include(x => x.Role)
            .AnyAsync(x =>
                x.UserId == userId &&
                body.RoleIds.Contains(x.RoleId) &&
                x.Role.Name != null &&
                x.Role.Name.ToLower() == "admin" &&
                ((normalizedProtectedEmail != null && x.User.Email != null && x.User.Email.ToLower() == normalizedProtectedEmail) ||
                 (normalizedProtectedUserName != null && x.User.UserName != null && x.User.UserName.ToLower() == normalizedProtectedUserName)));

        if (isProtectedAdminRoleRemoval)
        {
            return BadRequest(new MessageResponse("The Admin role of the .env dashboard admin user cannot be removed."));
        }

        var command = new DeleteUserRoleFullCommand(userId.ToString(), body.RoleIds.Select(x => x.ToString()).ToList());
        MessageResponse result = await _bus.InvokeAsync<MessageResponse>(command);
        return Ok(result);
    }

    [RoleFilter("Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(GiveUserRoleCommand request, CancellationToken cancellationToken)
    {
        var protectedEmail = _configuration["DashboardAdmin:Email"]?.Trim();
        var protectedUserName = _configuration["DashboardAdmin:UserName"]?.Trim();

        var targetUser = await _authService
            .GetAllUsers()
            .Where(x => x.Id == request.UserId)
            .Select(x => new { x.Email, x.UserName })
            .FirstOrDefaultAsync(cancellationToken);

        if (targetUser is null)
        {
            return BadRequest(new MessageResponse("User not found."));
        }

        var isProtectedDashboardAdmin =
            (!string.IsNullOrWhiteSpace(protectedEmail) &&
             string.Equals(targetUser.Email, protectedEmail, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(protectedUserName) &&
             string.Equals(targetUser.UserName, protectedUserName, StringComparison.OrdinalIgnoreCase));

        if (isProtectedDashboardAdmin)
        {
            return BadRequest(new MessageResponse("The protected .env dashboard admin user cannot receive additional roles."));
        }

        var targetRoleName = await _roleService
            .GetAllRoles()
            .Where(x => x.Id == request.RoleId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(targetRoleName))
        {
            return BadRequest(new MessageResponse("Role not found."));
        }

        if (string.Equals(targetRoleName, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new MessageResponse("The Admin role cannot be assigned from this screen."));
        }

        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }
}
