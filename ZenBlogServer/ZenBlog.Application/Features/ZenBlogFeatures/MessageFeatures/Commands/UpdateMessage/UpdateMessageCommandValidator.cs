using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;

public sealed class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
