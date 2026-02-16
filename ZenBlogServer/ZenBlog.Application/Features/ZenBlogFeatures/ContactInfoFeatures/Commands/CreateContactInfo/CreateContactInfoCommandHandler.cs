using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;

public sealed class CreateContactInfoCommandHandler
{
    private readonly IContactInfoService _contactInfoService;

    public CreateContactInfoCommandHandler(IContactInfoService contactInfoService)
    {
        _contactInfoService = contactInfoService;
    }

    public async Task<MessageResponse> Handle(CreateContactInfoCommand request, CancellationToken cancellationToken)
    {
        await _contactInfoService.CreateAsync(request, cancellationToken);
        return new MessageResponse("ContactInfo created successfully.");
    }
}
