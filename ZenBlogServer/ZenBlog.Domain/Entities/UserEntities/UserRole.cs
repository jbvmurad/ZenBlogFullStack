using System.ComponentModel.DataAnnotations.Schema;
using ZenBlog.Domain.Entities.Abstraction;

namespace ZenBlog.Domain.Entities.UserEntities;

public sealed class UserRole : BaseEntity
{
    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Role")]
    public string RoleId { get; set; }
    public Role Role { get; set; }

}
