using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.DeleteSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubCommentController :APIController
{
    private readonly ISubCommentService _subCommentService;
    private readonly IMapper _mapper;
    public SubCommentController(ISubCommentService subCommentService, IMessageBus bus,IMapper mapper) : base(bus)
    {
        _subCommentService= subCommentService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(ODataQueryOptions<SubComment> options, CancellationToken cancellationToken)
    {
        IQueryable<SubComment> query = _subCommentService.GetAllSubComments();

        query = (IQueryable<SubComment>)options.ApplyTo(query, new ODataQuerySettings());

        var result = await query
            .ProjectTo<SubCommentResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteSubCommentCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSubCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
