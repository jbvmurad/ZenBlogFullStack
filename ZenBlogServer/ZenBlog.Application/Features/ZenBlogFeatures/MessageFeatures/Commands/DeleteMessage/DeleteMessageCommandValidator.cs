using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.DeleteMessage;

public sealed class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
