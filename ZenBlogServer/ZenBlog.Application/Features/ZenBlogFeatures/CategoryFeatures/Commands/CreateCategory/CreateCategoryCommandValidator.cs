using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
