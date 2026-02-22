using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;

public sealed class DeleteContactInfoCommandHandler
{
    private readonly IContactInfoService _contactInfoService;

    public DeleteContactInfoCommandHandler(IContactInfoService contactInfoService)
    {
        _contactInfoService = contactInfoService;
    }

    public async Task<MessageResponse> Handle(DeleteContactInfoCommand request, CancellationToken cancellationToken)
    {
        await _contactInfoService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Contact info deleted successfully");
    }
}
