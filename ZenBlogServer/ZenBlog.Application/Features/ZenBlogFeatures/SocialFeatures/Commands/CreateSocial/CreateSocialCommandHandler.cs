using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;

public sealed class CreateSocialCommandHandler
{
    private readonly ISocialService _socialService;

    public CreateSocialCommandHandler(ISocialService socialService)
    {
        _socialService = socialService;
    }

    public async Task<MessageResponse> Handle(CreateSocialCommand request, CancellationToken cancellationToken)
    {
        await _socialService.CreateAsync(request, cancellationToken);
        return new MessageResponse("Social created successfully.");
    }
}
