using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using Microsoft.AspNetCore.Http;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface ISocialService
{
    IQueryable<Social>GetAllSocial();
    Task<Social?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<string> SaveSocialIconAsync(IFormFile media, CancellationToken cancellationToken);
    Task CreateAsync(CreateSocialCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateSocialCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteSocialCommand request, CancellationToken cancellationToken);
}
