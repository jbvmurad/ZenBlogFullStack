using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
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
    private readonly IMapper _mapper;

    public UserRoleController(IMessageBus bus, IUserRoleService userRoleService, IMapper mapper) : base(bus)
    {
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    [RoleFilter("Manager")]
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
        var command = new DeleteUserRoleFullCommand(userId.ToString(), body.RoleIds.Select(x => x.ToString()).ToList());
        MessageResponse result = await _bus.InvokeAsync<MessageResponse>(command);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GiveUserRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }
}
