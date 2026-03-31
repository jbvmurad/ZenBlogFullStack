using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ContactInfoController :APIController
{
    private readonly IContactInfoService _contactInfoService;
    private readonly IMapper _mapper;
    public ContactInfoController(IContactInfoService contactInfoService, IMessageBus bus,IMapper mapper) : base(bus)
    {
        _contactInfoService = contactInfoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(ODataQueryOptions<ContactInfo> options, CancellationToken cancellationToken)
    {
        IQueryable<ContactInfo> query = _contactInfoService.GetContactInfo();

        query = (IQueryable<ContactInfo>)options.ApplyTo(query, new ODataQuerySettings());

        var result = await query
            .ProjectTo<ContactInfoResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateContactInfoCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteContactInfoCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateContactInfoCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
