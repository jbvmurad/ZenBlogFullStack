using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface IContactInfoService
{
    IQueryable<ContactInfoResponse>GetContactInfo();
    Task CreateAsync(CreateContactInfoCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateContactInfoCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteContactInfoCommand request, CancellationToken cancellationToken);
}
