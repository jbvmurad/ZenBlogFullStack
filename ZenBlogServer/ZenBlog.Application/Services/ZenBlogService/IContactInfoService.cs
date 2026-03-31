using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface IContactInfoService
{
    IQueryable<ContactInfo>GetContactInfo();
    Task CreateAsync(CreateContactInfoCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateContactInfoCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteContactInfoCommand request, CancellationToken cancellationToken);
}
