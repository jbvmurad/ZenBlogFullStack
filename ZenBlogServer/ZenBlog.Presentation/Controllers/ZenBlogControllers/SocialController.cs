using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Application.Requests.SocialRequests;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SocialController :APIController
{
    private readonly ISocialService _socialService;
    public SocialController(ISocialService socialService, IMessageBus bus) : base(bus)
    {
        _socialService = socialService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Social> GetAll() => _socialService.GetAllSocial();

    [HttpPost]
    public async Task<IActionResult> Create(CreateSocialCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("with-media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateWithMedia(
        [FromForm] CreateSocialMediaOnlyRequest media,
        [FromQuery] string Title,
        [FromQuery] string Url,
        CancellationToken cancellationToken)
    {
        var command = media.ToCreateSocialWithMediaCommand(Title, Url);
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteSocialCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSocialCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("with-media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateWithMedia(
        [FromForm] UpdateSocialMediaOptionalRequest media,
        [FromQuery] string id,
        [FromQuery] string? Title,
        [FromQuery] string? Url,
        CancellationToken cancellationToken)
    {
        var command = media.ToUpdateSocialWithMediaCommand(id, Title, Url);
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(command, cancellationToken);
        return Ok(response);
    }
}
