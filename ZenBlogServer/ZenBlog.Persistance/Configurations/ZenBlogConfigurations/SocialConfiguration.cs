using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Persistance.Configurations.Abstraction;

namespace ZenBlog.Persistance.Configurations.ZenBlogConfigurations;

public sealed class SocialConfiguration : IEntityTypeConfiguration<Social>
{
    public void Configure(EntityTypeBuilder<Social> builder)
    {
        builder.ToTable("Socials");

        builder.ConfigureBaseEntity();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Icon)
            .HasMaxLength(200);
    }
}
