using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class SocialMap:Profile
{
    public SocialMap()
    {
        CreateMap<CreateSocialCommand, Social>().ReverseMap();
        CreateMap<UpdateSocialCommand, Social>().ReverseMap();
    }
}
