using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Persistance.Configurations.Abstraction;

namespace ZenBlog.Persistance.Configurations.ZenBlogConfigurations;

public sealed class SubCommentConfiguration : IEntityTypeConfiguration<SubComment>
{
    public void Configure(EntityTypeBuilder<SubComment> builder)
    {
        builder.ToTable("SubComments");

        builder.ConfigureBaseEntity();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.CommentId)
            .IsRequired()
            .HasMaxLength(36);

        builder.HasIndex(x => x.CommentId);

        builder.HasOne(x => x.Comment)
            .WithMany(x => x.SubComments)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
