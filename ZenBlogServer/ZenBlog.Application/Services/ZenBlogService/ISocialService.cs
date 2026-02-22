using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface ISocialService
{
    IQueryable<Social>GetAllSocial();

    Task CreateAsync(CreateSocialCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateSocialCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteSocialCommand request, CancellationToken cancellationToken);
}
