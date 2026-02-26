using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class CategoryMap:Profile
{
    public CategoryMap()
    {
        CreateMap<CreateCategoryCommand,Category>().ReverseMap();
        CreateMap<UpdateCategoryCommand,Category>().ReverseMap();
    }
}
