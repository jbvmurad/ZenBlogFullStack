using AutoMapper;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.RegisterUser;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUser;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Persistance.Mappings.UserMaps;

public sealed class AuthMap :Profile
{
    public AuthMap()
    {
        CreateMap<RegisterUserCommand, User>().ReverseMap();
        CreateMap<UpdateUserCommand, User>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        CreateMap<UserParameters, User>().ReverseMap();
        CreateMap<User, UserResponse>();
    }
}
