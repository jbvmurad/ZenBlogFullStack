using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Persistance.Configurations.UserAttributeConfigurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FullName)
            .IsRequired();

        builder.Property(u => u.ImageUrl);

        builder.HasMany(x => x.Blogs)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
