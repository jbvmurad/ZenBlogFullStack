using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.DeleteComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface ICommentService
{
    IQueryable<Comment>GetAllComments();
    Task CreateAsync(CreateCommentCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateCommentCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteCommentCommand request, CancellationToken cancellationToken);
}
