using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class BlogMap :Profile
{
    public BlogMap()
    {
        CreateMap<CreateBlogCommand,Blog>().ReverseMap();
        CreateMap<UpdateBlogCommand,Blog>().ReverseMap();
    }
}
