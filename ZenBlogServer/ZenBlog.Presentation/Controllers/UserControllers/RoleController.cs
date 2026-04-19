using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using ZenBlog.Application.Features.UserFeatures.RoleFeatures.Commands.CreateRole;
using ZenBlog.Application.Features.UserFeatures.RoleFeatures.Commands.DeleteRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Infrastructure.Authorization;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.UserControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoleController : APIController
{
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;

    public RoleController(IMessageBus bus, IRoleService roleService, IMapper mapper) : base(bus)
    {
        _roleService = roleService;
        _mapper = mapper;
    }

    [RoleFilter("Admin", "Manager")]
    [HttpGet]
    public async Task<IActionResult> GetAll(ODataQueryOptions<Role> options, CancellationToken cancellationToken)
    {
        IQueryable<Role> query = _roleService.GetAllRoles();

        query = (IQueryable<Role>)options.ApplyTo(query, new ODataQuerySettings());

        var result = await query
            .ProjectTo<RoleResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [RoleFilter("Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [RoleFilter("Admin")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetAllRoles().FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken);
        if (role?.Name != null && role.Name.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new MessageResponse("The Admin role cannot be deleted."));
        }

        DeleteRoleCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }
}
