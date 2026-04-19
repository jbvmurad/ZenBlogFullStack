using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class BlogMap :Profile
{
    public BlogMap()
    {
        CreateMap<CreateBlogCommand, Blog>()
            .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
            .ForMember(dest => dest.BlogImage, opt => opt.Ignore());

        CreateMap<UpdateBlogCommand, Blog>()
            .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
            .ForMember(dest => dest.BlogImage, opt => opt.Ignore());

        CreateMap<Blog, BlogResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));
    }
}
