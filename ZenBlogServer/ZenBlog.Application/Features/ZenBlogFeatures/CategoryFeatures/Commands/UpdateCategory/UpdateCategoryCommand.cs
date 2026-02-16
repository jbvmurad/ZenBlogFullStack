namespace ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    string Id,
    string CategoryName);
