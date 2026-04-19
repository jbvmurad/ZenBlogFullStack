using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.DeleteComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Infrastructure.Authorization;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CommentController :APIController
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public CommentController(
        ICommentService commentService,
        IAuthService authService,
        IMessageBus bus,
        IMapper mapper) : base(bus)
    {
        _commentService = commentService;
        _authService = authService;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(ODataQueryOptions<Comment> options, CancellationToken cancellationToken)
    {
        IQueryable<Comment> query = _commentService.GetAllComments();

        query = (IQueryable<Comment>)options.ApplyTo(query, new ODataQuerySettings());

        var result = await query
            .ProjectTo<CommentResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var commenterEmails = result
            .Select(x => x.Email)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (commenterEmails.Count > 0)
        {
            var commenterImages = await _authService
                .GetAllUsers()
                .Where(x => x.Email != null && commenterEmails.Contains(x.Email))
                .Select(x => new { x.Email, x.ImageUrl })
                .ToListAsync(cancellationToken);

            var imageLookup = commenterImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                .GroupBy(x => x.Email!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(item => item.ImageUrl).FirstOrDefault(),
                    StringComparer.OrdinalIgnoreCase);

            result = result
                .Select(item => item with
                {
                    CommenterImageUrl = imageLookup.TryGetValue(item.Email, out var imageUrl)
                        ? imageUrl
                        : null
                })
                .ToList();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [RoleFilter("Admin", "Manager")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteCommentCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [RoleFilter("Admin", "Manager")]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
