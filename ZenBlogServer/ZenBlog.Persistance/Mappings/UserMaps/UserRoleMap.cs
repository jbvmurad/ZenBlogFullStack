using AutoMapper;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Persistance.Mappings.UserMaps;

public sealed class UserRoleMap :Profile
{
    public UserRoleMap()
    {
        CreateMap<UserRole, UserRoleResponse>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
    }
}
