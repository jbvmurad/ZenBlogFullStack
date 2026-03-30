using AutoMapper;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Persistance.Mappings.UserMaps;

public sealed class RoleMap: Profile
{
    public RoleMap()
    {
        CreateMap<Role, RoleResponse>();
    }
}
