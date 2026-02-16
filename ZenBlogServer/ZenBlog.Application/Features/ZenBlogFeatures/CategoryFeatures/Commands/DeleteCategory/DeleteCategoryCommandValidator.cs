using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
