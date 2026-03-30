using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class SocialMap:Profile
{
    public SocialMap()
    {
        CreateMap<CreateSocialCommand, Social>()
            .ForMember(dest => dest.Icon, opt => opt.Ignore());

        CreateMap<UpdateSocialCommand, Social>()
            .ForMember(dest => dest.Icon, opt => opt.Ignore());

        CreateMap<Social, SocialResponse>();
    }
}
