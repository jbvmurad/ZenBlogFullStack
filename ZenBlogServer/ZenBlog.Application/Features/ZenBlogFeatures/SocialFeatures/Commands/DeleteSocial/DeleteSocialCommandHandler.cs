using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;

public sealed class DeleteSocialCommandHandler
{
    private readonly ISocialService _socialService;

    public DeleteSocialCommandHandler(ISocialService socialService)
    {
        _socialService = socialService;
    }

    public async Task<MessageResponse> Handle(DeleteSocialCommand request, CancellationToken cancellationToken)
    {
        await _socialService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Social deleted successfully");
    }
}
