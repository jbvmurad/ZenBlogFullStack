using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CategoryName).NotEmpty();
    }
}
