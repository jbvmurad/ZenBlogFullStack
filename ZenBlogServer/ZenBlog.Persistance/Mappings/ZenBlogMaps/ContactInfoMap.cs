using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class ContactInfoMap :Profile
{
    public ContactInfoMap()
    {
        CreateMap<CreateContactInfoCommand, ContactInfo>().ReverseMap();
        CreateMap<UpdateContactInfoCommand, ContactInfo>().ReverseMap();
    }
}
