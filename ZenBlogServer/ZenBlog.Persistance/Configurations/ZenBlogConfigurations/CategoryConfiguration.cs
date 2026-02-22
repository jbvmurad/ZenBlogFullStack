using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Persistance.Configurations.Abstraction;

namespace ZenBlog.Persistance.Configurations.ZenBlogConfigurations;


public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.ConfigureBaseEntity();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Blogs)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
