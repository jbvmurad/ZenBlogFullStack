using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.DeleteUserRole;
using ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.GiveUserRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.UserControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UserRoleController : APIController
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IMessageBus bus, IUserRoleService userRoleService) : base(bus)
    {
        _userRoleService = userRoleService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<UserRole> GetAll() => _userRoleService.GetAllUserRoles();

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteRoles([FromRoute] string userId, [FromBody] DeleteUserRoleBody body)
    {
        var command = new DeleteUserRoleFullCommand(userId.ToString(), body.RoleIds.Select(x => x.ToString()).ToList());
        MessageResponse result = await _bus.InvokeAsync<MessageResponse>(command);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GiveUserRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
