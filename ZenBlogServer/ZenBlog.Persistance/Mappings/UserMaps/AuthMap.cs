using AutoMapper;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.RegisterUser;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.UpdateUser;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Persistance.Mappings.UserMaps;

public sealed class AuthMap :Profile
{
    public AuthMap()
    {
        CreateMap<RegisterUserCommand,User>().ReverseMap();
        CreateMap<UpdateUserCommand,User>().ReverseMap();
        CreateMap<UserParameters,User>().ReverseMap();
    }
}
