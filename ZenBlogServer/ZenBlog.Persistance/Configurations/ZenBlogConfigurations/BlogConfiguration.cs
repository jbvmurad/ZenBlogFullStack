using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Persistance.Configurations.Abstraction;

namespace ZenBlog.Persistance.Configurations.ZenBlogConfigurations
{
    public sealed class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blogs");

            builder.ConfigureBaseEntity();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CoverImage)
                .HasMaxLength(500);

            builder.Property(x => x.BlogImage)
                .HasMaxLength(500);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(x => x.CategoryId)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(36);

            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.UserId);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Blogs)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Comments)
                .WithOne(x => x.Blog)
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
