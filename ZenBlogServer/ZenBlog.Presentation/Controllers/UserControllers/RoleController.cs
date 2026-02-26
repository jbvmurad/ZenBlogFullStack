using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.CreateRole;
using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.DeleteRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.UserControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoleController : APIController
{
    private readonly IRoleService _roleService;

    public RoleController(IMessageBus bus, IRoleService roleService) : base(bus)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Role> GetAll() => _roleService.GetAllRoles();

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteRoleCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
