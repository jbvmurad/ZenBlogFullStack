using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;

public sealed class UpdateContactInfoCommandHandler
{
    private readonly IContactInfoService _contactInfoService;

    public UpdateContactInfoCommandHandler(IContactInfoService contactInfoService)
    {
        _contactInfoService = contactInfoService;
    }

    public async Task<MessageResponse> Handle(UpdateContactInfoCommand request, CancellationToken cancellationToken)
    {
        await _contactInfoService.UpdateAsync(request, cancellationToken);
        return new MessageResponse("ContactInfo updated successfully.");
    }
}
