using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;

public sealed class UpdateSocialCommandHandler
{
    private readonly ISocialService _socialService;

    public UpdateSocialCommandHandler(ISocialService socialService)
    {
        _socialService = socialService;
    }

    public async Task<MessageResponse> Handle(UpdateSocialCommand request, CancellationToken cancellationToken)
    {
        await _socialService.UpdateAsync(request, cancellationToken);
        return new MessageResponse("Social updated successfully.");
    }
}
