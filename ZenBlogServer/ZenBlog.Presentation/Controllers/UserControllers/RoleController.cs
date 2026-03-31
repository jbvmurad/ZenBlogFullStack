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

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteRoleCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }
}
