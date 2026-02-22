using FluentValidation;
using FluentValidation.Results;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlogWithMedia;

public sealed class UpdateBlogWithMediaCommandHandler
{
    private readonly IBlogService _blogService;

    public UpdateBlogWithMediaCommandHandler(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<MessageResponse> Handle(UpdateBlogWithMediaCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogService.GetByIdAsync(request.Id, cancellationToken);
        if (blog is null)
            throw new ValidationException(new[] { new ValidationFailure("Id", "Blog not found") });

        var title = string.IsNullOrWhiteSpace(request.Title) ? blog.Title : request.Title!;
        var description = string.IsNullOrWhiteSpace(request.Description) ? blog.Description : request.Description!;
        var categoryId = string.IsNullOrWhiteSpace(request.CategoryId) ? blog.CategoryId : request.CategoryId!;
        var userId = string.IsNullOrWhiteSpace(request.UserId) ? blog.UserId : request.UserId!;

        var coverUrl = blog.CoverImage;
        if (request.CoverImage is not null)
            coverUrl = await _blogService.SaveBlogImageAsync(request.CoverImage, cancellationToken);

        var blogUrl = blog.BlogImage;
        if (request.BlogImage is not null)
            blogUrl = await _blogService.SaveBlogImageAsync(request.BlogImage, cancellationToken);

        var command = new UpdateBlogCommand(
            request.Id,
            title,
            coverUrl,
            blogUrl,
            description,
            categoryId,
            userId);

        await _blogService.UpdateAsync(command, cancellationToken);
        return new MessageResponse("Blog updated successfully");
    }
}
