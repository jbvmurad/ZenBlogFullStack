using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;

public sealed class CreateSubCommentCommandValidator : AbstractValidator<CreateSubCommentCommand>
{
    public CreateSubCommentCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.CommentId).NotEmpty();
    }
}
