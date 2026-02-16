using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;

public sealed class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.MessageBody).NotEmpty();
    }
}
