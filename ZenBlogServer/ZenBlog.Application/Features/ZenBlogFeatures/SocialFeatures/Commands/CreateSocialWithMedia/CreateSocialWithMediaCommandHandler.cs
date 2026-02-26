using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocialWithMedia;

public sealed class CreateSocialWithMediaCommandHandler
{
    private readonly ISocialService _socialService;

    public CreateSocialWithMediaCommandHandler(ISocialService socialService)
    {
        _socialService = socialService;
    }

    public async Task<MessageResponse> Handle(CreateSocialWithMediaCommand request, CancellationToken cancellationToken)
    {
        var iconUrl = await _socialService.SaveSocialIconAsync(request.Icon!, cancellationToken);

        var command = new CreateSocialCommand(
            request.Title!,
            request.Url!,
            iconUrl);

        await _socialService.CreateAsync(command, cancellationToken);
        return new MessageResponse("Social created successfully.");
    }
}
