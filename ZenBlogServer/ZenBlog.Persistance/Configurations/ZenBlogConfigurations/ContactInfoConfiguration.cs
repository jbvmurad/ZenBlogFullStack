using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Persistance.Configurations.Abstraction;

namespace ZenBlog.Persistance.Configurations.ZenBlogConfigurations;

public sealed class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
{
    public void Configure(EntityTypeBuilder<ContactInfo> builder)
    {
        builder.ToTable("ContactInfos");

        builder.ConfigureBaseEntity();

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.MapUrl)
            .HasMaxLength(1000);
    }
}
