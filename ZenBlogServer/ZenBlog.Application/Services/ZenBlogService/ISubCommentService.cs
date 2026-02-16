using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.DeleteSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface ISubCommentService
{
    IQueryable<SubComment> GetAllSubComments();
    Task CreateAsync(CreateSubCommentCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateSubCommentCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteSubCommentCommand request, CancellationToken cancellationToken);
}
