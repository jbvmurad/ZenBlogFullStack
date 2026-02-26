using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ContactInfoController :APIController
{
    private readonly IContactInfoService _contactInfoService;
    public ContactInfoController(IContactInfoService contactInfoService, IMessageBus bus) : base(bus)
    {
        _contactInfoService = contactInfoService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<ContactInfo> GetAll() => _contactInfoService.GetContactInfo();

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
