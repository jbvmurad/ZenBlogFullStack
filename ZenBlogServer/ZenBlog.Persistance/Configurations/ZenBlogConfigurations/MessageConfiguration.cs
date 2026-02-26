using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Persistance.Configurations.Abstraction;

namespace ZenBlog.Persistance.Configurations.ZenBlogConfigurations;


public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.ConfigureBaseEntity();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MessageBody)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => x.IsRead);
    }
}
