using FluentValidation;
using FluentValidation.Results;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUser;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUserWithMedia;

public sealed class UpdateUserWithMediaCommandHandler
{
    private readonly IAuthService _authService;

    public UpdateUserWithMediaCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(UpdateUserWithMediaCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            throw new ValidationException(new[] { new ValidationFailure("Id", "User not found") });

        var fullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName!;
        var email = string.IsNullOrWhiteSpace(request.Email) ? user.Email! : request.Email!;
        var phone = string.IsNullOrWhiteSpace(request.PhoneNumber) ? user.PhoneNumber : request.PhoneNumber;

        var imageUrl = user.ImageUrl;
        if (request.Image is not null)
            imageUrl = await _authService.SaveUserImageAsync(request.Image, cancellationToken);

        var command = new UpdateUserCommand(
            request.Id,
            fullName,
            email,
            phone,
            request.Password,
            imageUrl);

        await _authService.UpdateAsync(command, cancellationToken);
        return new MessageResponse("User updated successfully.");
    }
}
