using FluentValidation;
using FluentValidation.Results;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocialWithMedia;

public sealed class UpdateSocialWithMediaCommandHandler
{
    private readonly ISocialService _socialService;

    public UpdateSocialWithMediaCommandHandler(ISocialService socialService)
    {
        _socialService = socialService;
    }

    public async Task<MessageResponse> Handle(UpdateSocialWithMediaCommand request, CancellationToken cancellationToken)
    {
        var social = await _socialService.GetByIdAsync(request.Id, cancellationToken);
        if (social is null)
            throw new ValidationException(new[] { new ValidationFailure("Id", "Social record not found") });

        var title = string.IsNullOrWhiteSpace(request.Title) ? social.Title : request.Title!;
        var url = string.IsNullOrWhiteSpace(request.Url) ? social.Url : request.Url!;

        var iconUrl = social.Icon;
        if (request.Icon is not null)
            iconUrl = await _socialService.SaveSocialIconAsync(request.Icon, cancellationToken);

        var command = new UpdateSocialCommand(
            request.Id,
            title,
            url,
            iconUrl);

        await _socialService.UpdateAsync(command, cancellationToken);
        return new MessageResponse("Social updated successfully");
    }
}
